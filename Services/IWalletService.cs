using VitalityPortal.Models.Common;
using VitalityPortal.Models.Wallet;

namespace VitalityPortal.Services;

public interface IWalletService
{
    Task<OperationResult> RequestWithdrawalAsync(string userId, CreateWithdrawalRequest request);
}