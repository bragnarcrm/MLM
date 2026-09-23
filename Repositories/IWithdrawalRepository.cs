using VitalityPortal.Models.Wallet;

namespace VitalityPortal.Repositories;

public interface IWithdrawalRepository
{
    void Add(CreateWithdrawalRequest withdrawal);
}