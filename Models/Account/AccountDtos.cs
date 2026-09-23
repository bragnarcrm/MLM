using System.ComponentModel.DataAnnotations;

namespace VitalityPortal.Models.Account;

public sealed class RegisterAccountRequest
{
    [Required, StringLength(60)] public string Username { get; init; } = string.Empty;
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
    [Required, StringLength(60)] public string FirstName { get; init; } = string.Empty;
    [Required, StringLength(60)] public string LastName { get; init; } = string.Empty;
    [Required] public string Country { get; init; } = string.Empty;
    [Required, StringLength(30)] public string Mobile { get; init; } = string.Empty;
    [Required, MinLength(8)] public string Password { get; init; } = string.Empty;
    [Required] public string ConfirmPassword { get; init; } = string.Empty;
    public string? SponsorId { get; init; }
}

public sealed class ChangePasswordRequest
{
    [Required] public string OldPassword { get; init; } = string.Empty;
    [Required, MinLength(8)] public string NewPassword { get; init; } = string.Empty;
    [Required] public string ConfirmPassword { get; init; } = string.Empty;
}

public sealed class ChangeTransactionPasswordRequest
{
    [Required, MinLength(4)] public string TransactionPassword { get; init; } = string.Empty;
    [Required] public string ConfirmPassword { get; init; } = string.Empty;
}

public sealed record TransactionPasswordLogDto(int Id, string Action, string Remarks, string Status, string IpAddress, DateTime OccurredAt);

public sealed class ForgotPasswordRequest
{
    [Required] public string Username { get; init; } = string.Empty;
    [Required, EmailAddress] public string Email { get; init; } = string.Empty;
}

public sealed class ActivateAccountRequest
{
    [Required] public string PaymentMethod { get; init; } = string.Empty;
    [Range(90, 90)] public decimal Amount { get; init; }
}