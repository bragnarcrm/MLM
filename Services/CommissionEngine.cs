using VitalityPortal.Models.Portal;
using VitalityPortal.Repositories;

namespace VitalityPortal.Services;

public interface ICommissionEngine
{
    void ProcessPurchase(string buyerUserId, decimal amount);
}

// Centralizes VAT + payout math so every commission type (direct, level, rank, salary) is computed one way.
public static class CommissionMath
{
    public const decimal VatRate = 0.15m;
    public static decimal Vat(decimal amount) => Math.Round(amount * VatRate, 2, MidpointRounding.AwayFromZero);
}

public sealed class CommissionEngine(IPortalRepository repository) : ICommissionEngine
{
    public const decimal DirectReferralRate = 0.10m;
    private static readonly decimal[] LevelOverrideRates = [0.05m, 0.03m, 0.02m, 0.01m, 0.01m];

    public void ProcessPurchase(string buyerUserId, decimal amount)
    {
        if (amount <= 0) return;
        var chain = repository.GetSponsorChain(buyerUserId, LevelOverrideRates.Length + 1);
        for (var index = 0; index < chain.Count; index++)
        {
            var isDirect = index == 0;
            var rate = isDirect ? DirectReferralRate : (index - 1 < LevelOverrideRates.Length ? LevelOverrideRates[index - 1] : 0m);
            if (rate <= 0) continue;

            var ownerUserId = chain[index];
            var amountEarned = Math.Round(amount * rate, 2, MidpointRounding.AwayFromZero);
            var vat = CommissionMath.Vat(amountEarned);
            var name = isDirect ? "Direct Referral Bonus" : $"Level {index} Override Bonus";
            var category = isDirect ? "direct" : "level";

            repository.AddCommission(new CreateCommissionRequest(category, buyerUserId, name, amountEarned, vat, isDirect ? null : index, ownerUserId));
            repository.AddTransaction(ownerUserId, new WalletTransactionDto(DateOnly.FromDateTime(DateTime.UtcNow), "Commission", amountEarned + vat, name));
        }
    }
}
