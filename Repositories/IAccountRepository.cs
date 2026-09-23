using VitalityPortal.Models.Account;

namespace VitalityPortal.Repositories;

public interface IAccountRepository
{
    bool VerifyPassword(string userId, string password);
    void Register(string userId, string password);
    void ChangePassword(string userId, string password);
    void SetTransactionPassword(string userId, string password);
    AccountStatusDto GetStatus(string userId);
    void Activate(string userId);
}