public class User
{
    public required string ExternalId { get; set; }
    public ExternalSystem System { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public ICollection<Chat>? Chats { get; set; }
}

public enum ExternalSystem
{
    Stayntouch
}