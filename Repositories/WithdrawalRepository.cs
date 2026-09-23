using VitalityPortal.Models.Wallet;

namespace VitalityPortal.Repositories;

public sealed class WithdrawalRepository : IWithdrawalRepository
{
    private readonly List<CreateWithdrawalRequest> withdrawals = [];

    public void Add(CreateWithdrawalRequest withdrawal) => withdrawals.Add(withdrawal);
}