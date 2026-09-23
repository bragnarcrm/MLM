using Microsoft.AspNetCore.Identity;
using VitalityPortal.Data;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Wallet;
using VitalityPortal.Models.Portal;
using VitalityPortal.Repositories;

namespace VitalityPortal.Services;

public sealed class WalletService(
    IWithdrawalRepository withdrawalRepository,
    IPortalRepository portalRepository,
    UserManager<ApplicationUser> userManager,
    IPasswordHasher<ApplicationUser> passwordHasher) : IWalletService
{
    public async Task<OperationResult> RequestWithdrawalAsync(string userId, CreateWithdrawalRequest request)
    {
        var user = await userManager.FindByNameAsync(userId);
        if (user is null) return OperationResult.Failure("Account not found.");
        if (string.IsNullOrEmpty(user.TransactionPasswordHash)) return OperationResult.Failure("Set a transaction password before requesting a withdrawal.");
        if (passwordHasher.VerifyHashedPassword(user, user.TransactionPasswordHash, request.TransactionPassword) == PasswordVerificationResult.Failed)
            return OperationResult.Failure("Transaction password is incorrect.");

        var kyc = portalRepository.GetKycStatus(userId);
        if (kyc.Status != "Verified")
        {
            return OperationResult.Failure(kyc.Status == "Pending"
                ? "Your KYC documents are currently under compliance review. Payouts will unlock once verified."
                : "KYC verification required. Please upload your ID / Passport and Proof of Address in your Profile before requesting withdrawals.");
        }

        var earned = portalRepository.GetCommissions(userId, "all").Where(item => item.Status != "Pending").Sum(item => item.Total);
        var withdrawn = portalRepository.GetWithdrawals(userId).Where(item => item.Status != "Cancelled").Sum(item => item.Amount);
        var available = earned - withdrawn;
        if (request.Amount > available) return OperationResult.Failure($"Insufficient balance. Available balance is R{available:0.00}.");

        withdrawalRepository.Add(request);
        portalRepository.AddWithdrawal(userId, new WithdrawalDto(request.Amount, request.PaymentMethod, DateTime.UtcNow, "Pending"));
        return OperationResult.Success("Withdrawal request submitted.");
    }
}