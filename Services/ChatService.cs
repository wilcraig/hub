// chat history service interface
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public interface IChatService
{
    Task<List<Chat>> GetChatHistoryForUser(string userId);
    Task UpdateChatHistory(Chat chat);
    Task AddChatToHistory(Chat chat);
    Task<Chat?> GetChatByUserIds(List<string> userIds);
    Task<Chat> CreateChat(List<string> userIds);
}

// chat history service which retrieves chat history from the database for a specific user
public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public ChatService(ApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<Chat?> GetChatByUserIds(List<string> userIds)
    {
        return await _context.Chats
                             .Include(c => c.Users)
                             .Where(c => c.Users.All(u => userIds.Contains(u.ExternalId)) && c.Users.Count == userIds.Count)
                             .FirstOrDefaultAsync();
    }

   public async Task<Chat> CreateChat(List<string> userIds)
    {
        // Retrieve users from the database
        var users = _context.Users.Where(u => userIds.Contains(u.ExternalId)).ToList();

        // Check if all users are found
        if (users.Count != userIds.Count)
        {
            // Get or create missing users
            var externalUsers = await _userService.GetOrCreateUsers(userIds);
            
            // Combine users and external users
            var combinedUsers = users.Union(externalUsers).ToList();
            users = combinedUsers;
        }

        // Create the chat with the combined list of users
        var chat = new Chat
        {
            Users = users,
            Name = string.Join(",", users.Select(u => u.Name))
        };

        // Add the chat to the database
        await _context.Chats.AddAsync(chat);
        await _context.SaveChangesAsync();

        return chat;
    }

    public async Task<List<Chat>> GetChatHistoryForUser(string userId)
    {
        return await _context.Chats
                             .Include(c => c.Users)
                             .Where(c => c.Users.Any(u => u.ExternalId == userId))
                             .ToListAsync();
    }

    public async Task UpdateChatHistory(Chat chat)
    {
        // Update the chat history in the database
        _context.Chats.Update(chat);
        await _context.SaveChangesAsync();
    }

    public async Task AddChatToHistory(Chat chat)
    {
        // Add a new chat to the history in the database
        await _context.Chats.AddAsync(chat);
        await _context.SaveChangesAsync();
    }
}