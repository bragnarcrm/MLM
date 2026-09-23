using System.ComponentModel.DataAnnotations;

namespace VitalityPortal.Models.Auth;

public sealed class LoginRequest
{
    [Required] public string UserId { get; init; } = string.Empty;
    [Required] public string Password { get; init; } = string.Empty;
    [Required] public string Captcha { get; init; } = string.Empty;
}