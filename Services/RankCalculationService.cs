using Microsoft.EntityFrameworkCore;
using VitalityPortal.Data;
using VitalityPortal.Models.Portal;

namespace VitalityPortal.Services;

// Nightly job: recomputes rank tiers, pays rank-up bonuses, pays monthly team-sales salary,
// and promotes commissions from Pending to Approved once they clear a review hold period.
public sealed class RankCalculationService(IServiceScopeFactory scopeFactory, ILogger<RankCalculationService> logger) : BackgroundService
{
    public static readonly (string Rank, decimal MinPersonalSales, int MinActiveReferrals, decimal Incentive)[] Tiers =
    [
        ("Newbie", 0m, 0, 0m),
        ("Builder", 500m, 3, 100m),
        ("Leader", 2000m, 10, 500m),
        ("Ambassador", 5000m, 25, 1500m)
    ];

    public const decimal MonthlySalaryThreshold = 2000m;
    public const decimal MonthlySalaryAmount = 300m;
    public static readonly TimeSpan CommissionHoldPeriod = TimeSpan.FromDays(7);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try { await RunOnceAsync(); }
            catch (Exception ex) { logger.LogError(ex, "Rank calculation cycle failed."); }

            try { await Task.Delay(TimeSpan.FromHours(24), stoppingToken); }
            catch (TaskCanceledException) { }
        }
    }

    public async Task RunOnceAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortalDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<Repositories.IPortalRepository>();

        await RecalculateRanksAsync(context, repository);
        PayMonthlySalaries(context, repository);
        repository.ApproveMatureCommissions(CommissionHoldPeriod);
    }

    private static async Task RecalculateRanksAsync(PortalDbContext context, Repositories.IPortalRepository repository)
    {
        var users = await context.Users.ToListAsync();
        foreach (var user in users)
        {
            if (string.IsNullOrEmpty(user.UserName)) continue;
            var personalSales = context.Purchases.Where(p => p.UserId == user.UserName).Sum(p => (decimal?)(p.Amount * p.Quantity)) ?? 0m;
            var activeReferrals = context.Referrals.Count(r => r.SponsorId == user.UserName && r.Status == "Active");
            var newTier = Tiers.Last(tier => personalSales >= tier.MinPersonalSales && activeReferrals >= tier.MinActiveReferrals);

            if (newTier.Rank == user.Rank) continue;

            var oldRank = user.Rank;
            user.Rank = newTier.Rank;
            repository.AddRankHistory(user.UserName, new RankHistoryDto(oldRank, newTier.Rank, newTier.Incentive, DateOnly.FromDateTime(DateTime.UtcNow)));

            if (newTier.Incentive > 0)
            {
                var vat = CommissionMath.Vat(newTier.Incentive);
                repository.AddCommission(new CreateCommissionRequest("rank", user.UserName, $"{newTier.Rank} Rank Bonus", newTier.Incentive, vat, null, user.UserName));
                repository.AddTransaction(user.UserName, new WalletTransactionDto(DateOnly.FromDateTime(DateTime.UtcNow), "Rank Bonus", newTier.Incentive + vat, $"Promoted to {newTier.Rank}"));
            }
        }
        await context.SaveChangesAsync();
    }

    private static void PayMonthlySalaries(PortalDbContext context, Repositories.IPortalRepository repository)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var users = context.Users.ToList();
        foreach (var user in users)
        {
            if (string.IsNullOrEmpty(user.UserName)) continue;
            var teamSales = context.Referrals.Where(r => r.SponsorId == user.UserName).Sum(r => (decimal?)r.MonthlySales) ?? 0m;
            if (teamSales < MonthlySalaryThreshold) continue;

            var alreadyPaid = context.WalletTransactions.Any(t => t.UserId == user.UserName && t.Category == "Monthly Salary" && t.OccurredAt >= monthStart);
            if (alreadyPaid) continue;

            var vat = CommissionMath.Vat(MonthlySalaryAmount);
            repository.AddCommission(new CreateCommissionRequest("salary", user.UserName, "Monthly Team Salary", MonthlySalaryAmount, vat, null, user.UserName));
            repository.AddTransaction(user.UserName, new WalletTransactionDto(DateOnly.FromDateTime(DateTime.UtcNow), "Monthly Salary", MonthlySalaryAmount + vat, "Team sales threshold met"));
        }
    }
}
