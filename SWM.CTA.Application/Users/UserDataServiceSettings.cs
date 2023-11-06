using System.ComponentModel.DataAnnotations;

namespace SWM.CTA.Application.Users;

public class UserDataServiceSettings
{
    [Url]
    [Required]
    public string DataEndpoint { get; set; }
}