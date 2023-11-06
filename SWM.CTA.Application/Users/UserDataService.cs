using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

public class UserDataService : IUserDataService
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
        
        // Config Validation
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(_settings, new ValidationContext(_settings, null, null), results, true);
        if (results.Count > 0)
        {
            foreach (ValidationResult validationResult in results)
                _logger.LogError("Invalid configuration value for members '{Members}': {Reason}", 
                    string.Join(", ", validationResult.MemberNames), validationResult.ErrorMessage ?? "No reason provided.");
            throw new InvalidServiceConfigurationException(GetType(), _settings.GetType());
        }
        
    }

    #endregion

    #region Implementation

    public async Task<ICollection<User>> GetAll()
    {
        using HttpClient httpClient = new();
        try
        {
            // HTTP GET
            _logger.LogInformation("HTTP GET: {Url}", _settings.DataEndpoint);
            HttpResponseMessage response = await httpClient.GetAsync(_settings.DataEndpoint);

            // If non-success
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("HTTP Request failed with status code: {StatusCode}", response.StatusCode);
                return Array.Empty<User>();
            }

            // Read content as string
            string json = await response.Content.ReadAsStringAsync();
            
            // BUG: The received json may not be a valid structure. Let's apply a fix here so we can read it.
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