namespace pogadajmy_server.Dto
{
    public sealed class MessageDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
