using System.ComponentModel.DataAnnotations;

namespace VitalityPortal.Models.Profile;

public sealed class UpdateProfileRequest
{
    [Required] public string FirstName { get; init; } = string.Empty;
    [Required] public string LastName { get; init; } = string.Empty;
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
    [Required] public string Country { get; init; } = string.Empty;
    [Required] public string Mobile { get; init; } = string.Empty;
}