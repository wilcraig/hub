using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

//[Authorize]
public class ChatHub : Hub
{
    private static readonly ConcurrentDictionary<string, ConnectionState> _connections = new ConcurrentDictionary<string, ConnectionState>();
    private readonly ILogger<ChatHub> _logger; // Logger field
    private readonly IChatService _chatService; // Chat history service field

    // Modify the constructor to accept ILogger<ChatHub>
    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        // validate jwt here?
        string userId = Context.UserIdentifier;
        if(Context?.UserIdentifier == null)
        {
            var contextInfo = new
            {
                ConnectionId = Context.ConnectionId,
                UserIdentifier = Context.UserIdentifier,
                // Add other properties you need
            };
            _logger.LogWarning("User tried to connect with null identifier. OnConnectedAsync called.");
            string json = System.Text.Json.JsonSerializer.Serialize(contextInfo);
            _logger.LogInformation($"{json}");
            return Task.CompletedTask;
        }
        _connections[userId] = new ConnectionState
        {
            ConnectionId = Context.ConnectionId,
            Status = Status.Online
        };

        _logger.LogInformation($"User {userId} connected.");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string userId = Context.UserIdentifier;
        if(Context?.UserIdentifier == null)
        {
            _logger.LogWarning("User tried to connect with null identifier. OnDisconnectedAsync called.");
            return Task.CompletedTask;
        }
        _connections.TryRemove(userId, out _);

        return base.OnDisconnectedAsync(exception);
    }

    // Check the database to see if there's a chat with this group of users yet. If there is, return the chatId
    // If there is, return the chatId, then add or create a new group with that chat id
    // Get all the online users from the chat from the _connections dictionary keys, then add their connection ids to the group.
    // Create an object with the chatId and the name of the chat (from the db), + the chat summary object (from the db - should include last 10 messages + any other info needed)
    // Add the number of online users to the chat summary object
    // Send the chat summary object to the user
    public async Task InitializeGroup(List<string> userIds)
    {
        // Check if there is an existing chat with the given group of users
        var chat = await _chatService.GetChatByUserIds(userIds);

        // If no chat exists, create a new chat
        chat ??= await _chatService.CreateChat(userIds);
        var chatId = chat.Id.ToString();

        // Add or create a new group with the retrieved chat ID
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);

        // Get all the online users from the chat
        var onlineUsers = _connections.Keys.Where(userIds.Contains).ToList();

        // Add their connection IDs to the group
        onlineUsers.ForEach(async userId =>
        {
            var connectionId = _connections[userId].ConnectionId;
            await Groups.AddToGroupAsync(connectionId, chatId);
        });

        // Create the chat summary object
        var chatSummary = new ChatSummary
        {
            ChatId = chatId,
            ChatName = chat.Name,
            // Add other necessary chat summary information
            OnlineUserCount = onlineUsers.Count
        };

        // Send the chat summary object to the user
        await Clients.Caller.SendAsync("ReceiveChatSummary", chatSummary);
    }

    public async Task SendMessageToChat(string user, string message, string chatId)
    {
        // Send the message to the chat
        await Clients.Group(chatId).SendAsync("ReceiveMessage", user, message);
    }

    public async Task AddToGroup(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task GetAllConnectedUsers()
    {
        var usersInfo = _connections.Select(x => new
        {
            UserId = x.Key,
            Status = x.Value.Status
        });

        await Clients.All.SendAsync("ReceiveAllConnectedUsers", usersInfo);
    }
}

public class ConnectionState
{
    public string ConnectionId { get; set; }
    public Status Status { get; set; }
}

public enum Status
{
    Online,
    Offline,
    Busy,
    Away
}