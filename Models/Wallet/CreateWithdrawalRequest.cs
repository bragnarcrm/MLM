using System.ComponentModel.DataAnnotations;

namespace VitalityPortal.Models.Wallet;

public sealed class CreateWithdrawalRequest
{
    [Range(100, double.MaxValue)] public decimal Amount { get; init; }
    [Required] public string PaymentMethod { get; init; } = string.Empty;
    [Required] public string TransactionPassword { get; init; } = string.Empty;
}