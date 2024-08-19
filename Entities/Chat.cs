public class Chat
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Message>? Messages { get; set; }
    public DateTimeOffset LastUpdated {get;set;} = DateTimeOffset.Now;
    public required ICollection<User> Users { get; set; }
}