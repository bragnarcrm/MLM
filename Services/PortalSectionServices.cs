using Microsoft.AspNetCore.Identity;
using VitalityPortal.Data;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Portal;
using VitalityPortal.Repositories;

namespace VitalityPortal.Services;

public interface IDashboardService { Task<DashboardSummaryDto> GetSummaryAsync(string ownerId); }
public interface INetworkService { PaginatedDto<ReferralDto> GetReferrals(string ownerId, string? search, int page, int pageSize); TreeNodeDto GetTree(string userId); bool CanView(string ownerId, string targetUserId); IReadOnlyList<GenealogySearchResultDto> Search(string ownerId, string? query); TeamPerformanceDto GetTeamPerformance(string ownerId); OperationResult Reassign(string ownerId, string userId, string newSponsorId); BulkImportResultDto Import(string sponsorId, IReadOnlyList<ImportMemberRow> rows); }
public interface IIncomeService { IncomeSummaryDto GetSummary(string ownerId); PaginatedDto<CommissionDto> GetCommissions(string ownerId, string category, int page, int pageSize); IReadOnlyList<LevelTurnoverDto> GetLevelTurnover(string ownerId); IReadOnlyList<RankHistoryDto> GetRankHistory(string ownerId); }
public interface IWalletActivityService { WalletBalanceDto GetBalance(string userId); PaginatedDto<WalletTransactionDto> GetTransactions(string userId, int page, int pageSize); PaginatedDto<WithdrawalDto> GetWithdrawals(string userId, int page, int pageSize); PayoutSettingsDto GetPayoutSettings(); OperationResult SavePayoutSettings(PayoutSettingsRequest request); OperationResult CancelWithdrawal(string userId, int id); }
public interface ICommerceService { PaginatedDto<PurchaseDto> GetPurchases(string userId, int page, int pageSize); PaginatedDto<OrderDto> GetOrders(string userId, int page, int pageSize); void RecordPurchase(string userId, CreatePurchaseRequest request); void RecordOrder(string userId, CreateOrderRequest request); }
public interface ICommunicationService {
    PaginatedDto<PortalMessageDto> GetInbox(string userId, int page, int pageSize);
    PaginatedDto<PortalMessageDto> GetSent(string userId, int page, int pageSize);
    OperationResult Send(string senderUserId, string senderName, SendMessageRequest request);
    OperationResult MarkAsRead(string userId, int messageId);
    OperationResult Delete(string userId, int messageId);
    IReadOnlyList<ZoomMeetingDto> GetMeetings();
}

public sealed class DashboardService(IPortalRepository repository, UserManager<ApplicationUser> userManager) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(string ownerId)
    {
        var withdrawals = repository.GetWithdrawals(ownerId);
        var referrals = repository.GetReferrals().Where(item => item.SponsorId == ownerId).ToList();
        var balance = repository.GetCommissions(ownerId, "all");
        var earned = balance.Where(item => item.Status != "Pending").Sum(item => item.Total);
        var withdrawn = withdrawals.Where(item => item.Status != "Cancelled").Sum(item => item.Amount);
        var user = await userManager.FindByNameAsync(ownerId);
        return new DashboardSummaryDto(earned, withdrawals.Sum(item => item.Amount), earned - withdrawn, referrals.Count, user?.Rank ?? "Newbie", referrals, withdrawals.Where(item => item.Status == "Pending").ToList());
    }
}

public sealed class NetworkService(IPortalRepository repository) : INetworkService
{
    public PaginatedDto<ReferralDto> GetReferrals(string ownerId, string? search, int page, int pageSize)
    {
        var query = repository.GetReferrals().Where(item => item.SponsorId == ownerId).Where(item => string.IsNullOrWhiteSpace(search) || item.UserId.Contains(search, StringComparison.OrdinalIgnoreCase) || item.FullName.Contains(search, StringComparison.OrdinalIgnoreCase));
        return Pagination.Page(query, page, pageSize);
    }

    public TreeNodeDto GetTree(string userId) => repository.GetTree(userId);
    public bool CanView(string ownerId, string targetUserId) => repository.IsInDownline(ownerId, targetUserId);
    public IReadOnlyList<GenealogySearchResultDto> Search(string ownerId, string? query) => repository.SearchDownline(ownerId, query);
    public TeamPerformanceDto GetTeamPerformance(string ownerId) => repository.GetTeamPerformance(ownerId);
    public OperationResult Reassign(string ownerId, string userId, string newSponsorId)
    {
        if (!repository.IsInDownline(ownerId, userId)) return OperationResult.Failure("You can only reassign members in your own team.");
        if (!repository.IsInDownline(ownerId, newSponsorId)) return OperationResult.Failure("The new sponsor must also be in your team.");
        return repository.ReassignReferral(userId, newSponsorId) ? OperationResult.Success("Member reassigned successfully.") : OperationResult.Failure("Could not reassign that member.");
    }
    public BulkImportResultDto Import(string sponsorId, IReadOnlyList<ImportMemberRow> rows) => repository.ImportMembers(sponsorId, rows);
}

public sealed class IncomeService(IPortalRepository repository) : IIncomeService
{
    public IncomeSummaryDto GetSummary(string ownerId)
    {
        var commissions = repository.GetCommissions(ownerId, "all");
        decimal Total(string category) => commissions.Where(item => item.Name.Contains(category, StringComparison.OrdinalIgnoreCase)).Sum(item => item.Total);
        return new IncomeSummaryDto(commissions.Sum(item => item.Total), Total("Direct Referral"), Total("Level"), Total("Rank"), Total("Level Turnover"), Total("Monthly Salary"));
    }
    public PaginatedDto<CommissionDto> GetCommissions(string ownerId, string category, int page, int pageSize) => Pagination.Page(repository.GetCommissions(ownerId, category), page, pageSize);
    public IReadOnlyList<LevelTurnoverDto> GetLevelTurnover(string ownerId) => repository.GetLevelTurnover(ownerId);
    public IReadOnlyList<RankHistoryDto> GetRankHistory(string ownerId) => repository.GetRankHistory(ownerId);
}

public sealed class WalletActivityService(IPortalRepository repository) : IWalletActivityService
{
    public WalletBalanceDto GetBalance(string userId)
    {
        var commissions = repository.GetCommissions(userId, "all");
        var earned = commissions.Where(item => item.Status != "Pending").Sum(item => item.Total);
        var pending = commissions.Where(item => item.Status == "Pending").Sum(item => item.Total);
        var withdrawn = repository.GetWithdrawals(userId).Where(item => item.Status != "Cancelled").Sum(item => item.Amount);
        return new WalletBalanceDto(0m, earned, withdrawn, earned - withdrawn, pending);
    }

    public PaginatedDto<WalletTransactionDto> GetTransactions(string userId, int page, int pageSize) => Pagination.Page(repository.GetTransactions(userId), page, pageSize);
    public PaginatedDto<WithdrawalDto> GetWithdrawals(string userId, int page, int pageSize) => Pagination.Page(repository.GetWithdrawals(userId), page, pageSize);
    public PayoutSettingsDto GetPayoutSettings() => repository.GetPayoutSettings();
    public OperationResult SavePayoutSettings(PayoutSettingsRequest request)
    {
        repository.SavePayoutSettings(new PayoutSettingsDto(request.AccountHolder, request.AccountNumber, request.BankName, request.AccountType, request.Address, request.Country, request.PostalCode));
        return OperationResult.Success("Payout settings saved.");
    }
    public OperationResult CancelWithdrawal(string userId, int id) =>
        repository.CancelWithdrawal(userId, id) ? OperationResult.Success("Withdrawal request cancelled.") : OperationResult.Failure("Withdrawal request could not be cancelled.");
}

public sealed class CommerceService(IPortalRepository repository, ICommissionEngine commissionEngine) : ICommerceService
{
    public PaginatedDto<PurchaseDto> GetPurchases(string userId, int page, int pageSize) => Pagination.Page(repository.GetPurchases(userId), page, pageSize);
    public PaginatedDto<OrderDto> GetOrders(string userId, int page, int pageSize) => Pagination.Page(repository.GetOrders(userId), page, pageSize);
    public void RecordPurchase(string userId, CreatePurchaseRequest request)
    {
        repository.AddPurchase(userId, request);
        commissionEngine.ProcessPurchase(userId, request.Amount * request.Quantity);
    }
    public void RecordOrder(string userId, CreateOrderRequest request) => repository.AddOrder(userId, request);
}

public sealed class CommunicationService(IPortalRepository repository) : ICommunicationService
{
    public PaginatedDto<PortalMessageDto> GetInbox(string userId, int page, int pageSize) => Pagination.Page(repository.GetInbox(userId), page, pageSize);
    public PaginatedDto<PortalMessageDto> GetSent(string userId, int page, int pageSize) => Pagination.Page(repository.GetSentMessages(userId), page, pageSize);
    public OperationResult Send(string senderUserId, string senderName, SendMessageRequest request) => repository.AddMessage(senderUserId, senderName, request);
    public OperationResult MarkAsRead(string userId, int messageId) => repository.MarkMessageAsRead(userId, messageId);
    public OperationResult Delete(string userId, int messageId) => repository.DeleteMessage(userId, messageId);
    public IReadOnlyList<ZoomMeetingDto> GetMeetings() => repository.GetMeetings();
}

internal static class Pagination
{
    public static PaginatedDto<T> Page<T>(IEnumerable<T> source, int page, int pageSize)
    {
        var safePage = Math.Max(page, 1);
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        var items = source.ToList();
        return new PaginatedDto<T>(items.Skip((safePage - 1) * safePageSize).Take(safePageSize).ToList(), safePage, safePageSize, items.Count);
    }
}