using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SWM.CTA.Domain.Users;
using SWM.CTA.Infrastructure.Services;

namespace SWM.CTA.Application.Users;

public interface IUserDataService
{
    Task<ICollection<User>> GetAll();
    Task<User?> GetById(long id);
}

/// <summary>Provides access to user data.</summary>
public class UserDataService : IUserDataService, IConfigurableService
{
    private readonly UserDataServiceSettings _settings;
    private readonly ILogger<UserDataService> _logger;

    #region Construction

    public UserDataService(
        IOptions<UserDataServiceSettings> settings,
        ILogger<UserDataService> logger
    )
    {
        _settings = settings.Value;
        _logger = logger;
    }

    #endregion

    #region Implementation

    public async Task<ICollection<User>> GetAll()
    {
        using HttpClient httpClient = new();
        try
        {
            // HTTP GET
            httpClient.Timeout = TimeSpan.FromSeconds(_settings.DataEndpointTimeout);
            _logger.LogInformation("HTTP GET: {Url} (Timeout: {Timeout})", _settings.DataEndpoint, httpClient.Timeout);
            HttpResponseMessage response = await httpClient.GetAsync(_settings.DataEndpoint);

            // If non-success
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP Request failed with status code: {StatusCode}", response.StatusCode);
                return Array.Empty<User>();
            }

            // Read content as string
            string json = await response.Content.ReadAsStringAsync();
            
            // BUGFIX: The received json may not be a valid structure. Let's apply a fix here so we can read it.
            if (_settings.FixInvalidJson)
                json = json.Replace(", gender:\"", ", \"gender\":\"");
            
            // Deserialize to target type
            List<User> users = JsonSerializer.Deserialize<List<User>>(json, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<User>();
            
            _logger.LogInformation("Successfully read {Count} users", users.Count);
            return users;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user data from endpoint {Endpoint}", _settings.DataEndpoint);
            return Array.Empty<User>();
        }
    }

    public async Task<User?> GetById(long id) => (await GetAll()).SingleOrDefault(u => u.Id == id);

    #endregion
}