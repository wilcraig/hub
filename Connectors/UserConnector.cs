using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public interface IUserConnector
{
    Task<User?> GetUserAsync(string userId);
}

public class UserConnector : IUserConnector
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserConnector> _logger;

    public UserConnector(HttpClient httpClient, ILogger<UserConnector> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<User?> GetUserAsync(string userId)
    {
        var response = await _httpClient.GetAsync($"https://localhost:3000/api/users/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to get user with ID {userId} from PMS. Status code: {response.StatusCode}");
            return null;
        }

        var userJson = await response.Content.ReadAsStringAsync();
        var pmsUser = JsonConvert.DeserializeObject<JObject>(userJson);

        if (pmsUser == null || pmsUser["id"] == null || pmsUser["full_name"] == null || pmsUser["email"] == null)
        {
            _logger.LogError($"Failed to deserialize user with ID {userId} from PMS. Invalid user data.");
            return null;
        }

        var user = new User
        {
            ExternalId = pmsUser["id"].Value<string>(),
            Name = pmsUser["full_name"].Value<string>(),
            Email = pmsUser["email"].Value<string>(),
            System = ExternalSystem.Stayntouch
        };

        return user;
    }
}