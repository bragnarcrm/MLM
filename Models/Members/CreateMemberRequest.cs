using System.ComponentModel.DataAnnotations;

namespace VitalityPortal.Models.Members;

public sealed class CreateMemberRequest
{
    [Required] public string Username { get; init; } = string.Empty;
    [Required] public string FirstName { get; init; } = string.Empty;
    [Required] public string LastName { get; init; } = string.Empty;
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
    [Required] public string Country { get; init; } = string.Empty;
    [Required] public string Mobile { get; init; } = string.Empty;
    [Required, MinLength(8)] public string Password { get; init; } = string.Empty;
    [Required] public string ConfirmPassword { get; init; } = string.Empty;
}

public sealed class UpdateReferralRequest
{
    [Required] public string FullName { get; init; } = string.Empty;
    [Required] public string Mobile { get; init; } = string.Empty;
    [Required] public string Rank { get; init; } = string.Empty;
    public decimal MonthlySales { get; init; }
    [Required] public string Status { get; init; } = string.Empty;
}