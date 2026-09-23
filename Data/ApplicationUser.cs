using Microsoft.AspNetCore.Identity;

namespace VitalityPortal.Data;

public sealed class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsActivated { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public string? TransactionPasswordHash { get; set; }
    public string Rank { get; set; } = "Newbie";
}