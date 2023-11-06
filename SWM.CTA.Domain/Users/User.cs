namespace SWM.CTA.Domain.Users;

/// <summary>Domain model for a single user.</summary>
public class User
{
    public long Id { get; set; }
    
    public string First { get; set; }
    public string Last { get; set; }
    
    public int Age { get; set; }
    public string Gender { get; set; }
}