using VitalityPortal.Models.Account;

namespace VitalityPortal.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly Dictionary<string, string> passwords = new(StringComparer.OrdinalIgnoreCase) { ["samkelisojam"] = "password1" };
    private readonly Dictionary<string, string> transactionPasswords = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, AccountStatusDto> statuses = new(StringComparer.OrdinalIgnoreCase);

    public bool VerifyPassword(string userId, string password) => passwords.TryGetValue(userId, out var savedPassword) && savedPassword == password;
    public void Register(string userId, string password) => passwords[userId] = password;
    public void ChangePassword(string userId, string password) => passwords[userId] = password;
    public void SetTransactionPassword(string userId, string password) => transactionPasswords[userId] = password;
    public AccountStatusDto GetStatus(string userId) => statuses.TryGetValue(userId, out var status) ? status : new AccountStatusDto(false, null);
    public void Activate(string userId) => statuses[userId] = new AccountStatusDto(true, DateTime.UtcNow);
}