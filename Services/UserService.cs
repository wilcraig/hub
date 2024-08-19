using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public interface IUserService
{
    Task<List<User>> GetOrCreateUsers(List<string> userIds);
}

public class UserService : IUserService
{
    private readonly IUserConnector _userConnector;
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context, IUserConnector userConnector)
    {
        _context = context;
        _userConnector = userConnector;
    }

    public async Task<List<User>> GetOrCreateUsers(List<string> userIds)
    {
        var users = new ConcurrentBag<User>();

        var alreadyExistingUsers = await _context.Users.Where(u => userIds.Contains(u.ExternalId)).ToListAsync();

        var tasks = new List<Task>();

        foreach (string userId in userIds.Except(alreadyExistingUsers.Select(u => u.ExternalId)))
        {
            tasks.Add(Task.Run(async () =>
            {
                var user = await _userConnector.GetUserAsync(userId) ?? throw new UserNotFoundException($"User with ID {userId} not found in PMS");
                _context.Users.Add(user);
                users.Add(user);
            }));
        }

        await Task.WhenAll(tasks);
        await _context.SaveChangesAsync();

        return users.ToList();
    }
}

[Serializable]
internal class UserNotFoundException : Exception
{
    public UserNotFoundException()
    {
    }

    public UserNotFoundException(string? message) : base(message)
    {
    }

    public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}