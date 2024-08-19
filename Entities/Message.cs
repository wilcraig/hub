public class Message
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public User Sender { get; set; }
    public string ReactionData {get;set;}
    public ICollection<User> ReadBy { get; set; }
    public Chat Chat { get; set; }
}