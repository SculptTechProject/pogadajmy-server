using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pogadajmy_server.Dto;
using pogadajmy_server.Infrastructure;
using pogadajmy_server.Models;
using pogadajmy_server.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace pogadajmy_server.Controllers
{
    [ApiController, Route("api/v1/rooms"), Authorize]
    public sealed class RoomsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RoomsController(AppDbContext db) => _db = db;

        private Guid UserId
        {
            get
            {
                var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(sub, out var id))
                    throw new UnauthorizedAccessException("Missing or invalid 'sub' claim.");

                return id;
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            var r = new Room { Type = dto.Type, Name = dto.Name, IsPrivate = dto.Type == "dm" };
            _db.Rooms.Add(r);
            _db.RoomMembers.Add(new RoomMember { RoomId = r.Id, UserId = UserId });
            await _db.SaveChangesAsync();
            return Ok(new { r.Id, r.Type, r.Name });
        }
        
        [Authorize]
        [HttpPost("{id:guid}/join")]
        public async Task<IActionResult> Join([FromRoute] Guid id)
        {
            var exists = await _db.Rooms.AnyAsync(x => x.Id == id);
            if (!exists) return NotFound();
            var already = await _db.RoomMembers.FindAsync(id, UserId);
            if (already is null)
            {
                _db.RoomMembers.Add(new RoomMember { RoomId = id, UserId = UserId });
                await _db.SaveChangesAsync();
            }
            return NoContent();
        }
        
        [Authorize]
        [HttpGet("{id:guid}/messages")]
        public async Task<IActionResult> History(Guid id, int take = 50, DateTime? before = null)
        {
            var uid = User.GetSubGuid();
            var isMember = await _db.RoomMembers.FindAsync(id, uid) is not null;
            if (!isMember) return Forbid();

            var q = _db.Messages.AsNoTracking().Where(m => m.RoomId == id);
            if (before.HasValue) q = q.Where(m => m.CreatedAt < before.Value);

            var items = await q.OrderByDescending(m => m.CreatedAt)
                .Take(Math.Clamp(take, 1, 200))
                .ToListAsync();
            return Ok(items.OrderBy(m => m.CreatedAt)); // zwracaj rosnąco
        }
        
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> MyRooms()
        {
            var uid = User.GetSubGuid(); // zrób helpera ClaimsPrincipal -> Guid
            var q = _db.RoomMembers
                .AsNoTracking()
                .Where(m => m.UserId == uid)
                .Select(m => new {
                    id = m.RoomId,
                    name = m.Room!.Name,
                    type = m.Room.Type,
                    members = _db.RoomMembers.Count(x => x.RoomId == m.RoomId),
                    lastMessageAt = _db.Messages
                        .Where(x => x.RoomId == m.RoomId)
                        .OrderByDescending(x => x.CreatedAt)
                        .Select(x => (DateTime?)x.CreatedAt)
                        .FirstOrDefault()
                });

            return Ok(await q.ToListAsync());
        }
        
        [HttpPost("dm")]
        public async Task<IActionResult> CreateDm([FromBody] Guid otherUserId)
        {
            var me = User.GetSubGuid();
            if (me == otherUserId) return BadRequest("cannot_dm_self");

            // reuse: DM room unique per pair (ordered key)
            var exists = await _db.DmRooms.FirstOrDefaultAsync(x =>
                (x.UserA == me && x.UserB == otherUserId) ||
                (x.UserA == otherUserId && x.UserB == me));

            if (exists is not null) return Ok(new { id = exists.RoomId });

            var room = new Room { Id = Guid.NewGuid(), Type = "dm", Name = null };
            _db.Rooms.Add(room);
            _db.RoomMembers.AddRange(
                new RoomMember { RoomId = room.Id, UserId = me },
                new RoomMember { RoomId = room.Id, UserId = otherUserId }
            );
            _db.DmRooms.Add(new DmRoom { RoomId = room.Id, UserA = me, UserB = otherUserId });
            await _db.SaveChangesAsync();
            return Ok(new { id = room.Id });
        }
    }
}
