namespace pogadajmy_server.Dto
{
    public sealed record CreateRoomDto(string Type, string? Name); // "dm"|"group"
    public sealed record JoinRoomDto(Guid RoomId);
    public sealed record SendMessageDto(Guid RoomId, string Text);
}
