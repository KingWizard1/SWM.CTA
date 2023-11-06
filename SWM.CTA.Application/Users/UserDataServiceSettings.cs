using System.ComponentModel.DataAnnotations;
using SWM.CTA.Infrastructure.Services;

namespace SWM.CTA.Application.Users;

/// <summary>Configuration object for the <see cref="UserDataService"/>.</summary>
public class UserDataServiceSettings : IServiceConfiguration
{
    [Url]
    [Required]
    public string DataEndpoint { get; set; }

    [Range(5, 60)]
    public int DataEndpointTimeout { get; set; } = 15;

    public bool FixInvalidJson { get; set; }

}