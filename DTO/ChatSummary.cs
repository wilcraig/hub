using System.ComponentModel.DataAnnotations;

public class ChatSummary
{
    public required string ChatId { get; set; }
    public required string ChatName { get; set; }
    public int OnlineUserCount { get; set; }
}