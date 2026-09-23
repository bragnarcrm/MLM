using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Portal;
using VitalityPortal.Repositories;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController, Authorize, Route("api/dashboard")]
public sealed class DashboardController(IDashboardService service, IUserContextService users) : ControllerBase
{
    [HttpGet("summary")] public async Task<ActionResult<DashboardSummaryDto>> Summary() => Ok(await service.GetSummaryAsync(users.GetRequiredUserId(User)));
}

[ApiController, Authorize, Route("api/network")]
public sealed class NetworkController(INetworkService service, IUserContextService users, RankCalculationService rankJob) : ControllerBase
{
    [HttpGet("referrals")] public ActionResult<PaginatedDto<ReferralDto>> Referrals([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetReferrals(users.GetRequiredUserId(User), search, page, pageSize));
    [HttpGet("tree/{userId}")] public ActionResult<TreeNodeDto> Tree(string userId)
    {
        var callerId = users.GetRequiredUserId(User);
        return service.CanView(callerId, userId) ? Ok(service.GetTree(userId)) : Forbid();
    }
    [HttpGet("search")] public ActionResult<IReadOnlyList<GenealogySearchResultDto>> Search([FromQuery] string? query) => Ok(service.Search(users.GetRequiredUserId(User), query));
    [HttpGet("team-performance")] public ActionResult<TeamPerformanceDto> TeamPerformance() => Ok(service.GetTeamPerformance(users.GetRequiredUserId(User)));
    [HttpPut("referrals/{userId}/reassign")] public ActionResult<OperationResult> Reassign(string userId, ReassignReferralRequest request)
    {
        var result = service.Reassign(users.GetRequiredUserId(User), userId, request.NewSponsorId);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
    [HttpPost("import")] public ActionResult<BulkImportResultDto> Import(IReadOnlyList<ImportMemberRow> rows) => Ok(service.Import(users.GetRequiredUserId(User), rows));

    // Dev/testing convenience: the real cycle runs automatically every 24h via RankCalculationService.
    [HttpPost("recalculate-ranks")] public async Task<ActionResult<OperationResult>> RecalculateRanks() { await rankJob.RunOnceAsync(); return Ok(OperationResult.Success("Rank, salary, and commission approval cycle completed.")); }
}

[ApiController, Authorize, Route("api/income")]
public sealed class IncomeController(IIncomeService service, IUserContextService users) : ControllerBase
{
    [HttpGet("summary")] public ActionResult<IncomeSummaryDto> Summary() => Ok(service.GetSummary(users.GetRequiredUserId(User)));
    [HttpGet("commissions/{category}")] public ActionResult<PaginatedDto<CommissionDto>> Commissions(string category, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetCommissions(users.GetRequiredUserId(User), category, page, pageSize));
    [HttpGet("level-turnover")] public ActionResult<IReadOnlyList<LevelTurnoverDto>> LevelTurnover() => Ok(service.GetLevelTurnover(users.GetRequiredUserId(User)));
    [HttpGet("rank-history")] public ActionResult<IReadOnlyList<RankHistoryDto>> RankHistory() => Ok(service.GetRankHistory(users.GetRequiredUserId(User)));
}

[ApiController, Authorize, Route("api/wallet")]
public sealed class WalletActivityController(IWalletActivityService service, IUserContextService users) : ControllerBase
{
    [HttpGet("balance")] public ActionResult<WalletBalanceDto> Balance() => Ok(service.GetBalance(users.GetRequiredUserId(User)));
    [HttpGet("transactions")] public ActionResult<PaginatedDto<WalletTransactionDto>> Transactions([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetTransactions(users.GetRequiredUserId(User), page, pageSize));
    [HttpGet("withdrawals")] public ActionResult<PaginatedDto<WithdrawalDto>> Withdrawals([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetWithdrawals(users.GetRequiredUserId(User), page, pageSize));
    [HttpGet("payout-settings")] public ActionResult<PayoutSettingsDto> GetPayoutSettings() => Ok(service.GetPayoutSettings());
    [HttpPut("payout-settings")] public ActionResult<OperationResult> SavePayoutSettings(PayoutSettingsRequest request) => Ok(service.SavePayoutSettings(request));
    [HttpPut("withdrawals/{id:int}/cancel")] public ActionResult<OperationResult> CancelWithdrawal(int id)
    {
        var result = service.CancelWithdrawal(users.GetRequiredUserId(User), id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}

[ApiController, Authorize, Route("api/commerce")]
public sealed class CommerceController(ICommerceService service, IUserContextService users) : ControllerBase
{
    [HttpGet("purchases")] public ActionResult<PaginatedDto<PurchaseDto>> Purchases([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetPurchases(users.GetRequiredUserId(User), page, pageSize));
    [HttpGet("orders")] public ActionResult<PaginatedDto<OrderDto>> Orders([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetOrders(users.GetRequiredUserId(User), page, pageSize));
}

[ApiController, Authorize, Route("api/communications")]
public sealed class CommunicationsController(ICommunicationService service, IUserContextService users) : ControllerBase
{
    [HttpGet("inbox")] public ActionResult<PaginatedDto<PortalMessageDto>> Inbox([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetInbox(users.GetRequiredUserId(User), page, pageSize));
    [HttpGet("sent")] public ActionResult<PaginatedDto<PortalMessageDto>> Sent([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetSent(users.GetRequiredUserId(User), page, pageSize));
    [HttpPost("messages")] public ActionResult<OperationResult> Send(SendMessageRequest request) => Ok(service.Send(users.GetRequiredUserId(User), users.GetRequiredUserId(User), request));
    [HttpPut("messages/{id:int}/read")] public ActionResult<OperationResult> MarkAsRead(int id) => Ok(service.MarkAsRead(users.GetRequiredUserId(User), id));
    [HttpDelete("messages/{id:int}")] public ActionResult<OperationResult> Delete(int id) => Ok(service.Delete(users.GetRequiredUserId(User), id));
    [HttpGet("meetings")] public ActionResult<IReadOnlyList<ZoomMeetingDto>> Meetings() => Ok(service.GetMeetings());
}

[ApiController, Authorize, Route("api/commissions")]
public sealed class CommissionsController(IPortalRepository repository) : ControllerBase
{
    [HttpPost] public ActionResult<OperationResult> Create(CreateCommissionRequest request)
    {
        repository.AddCommission(request);
        return Ok(OperationResult.Success("Commission recorded."));
    }
}

[ApiController, Authorize, Route("api/purchases")]
public sealed class PurchasesController(ICommerceService service, IUserContextService users) : ControllerBase
{
    [HttpPost] public ActionResult<OperationResult> Create(CreatePurchaseRequest request)
    {
        service.RecordPurchase(users.GetRequiredUserId(User), request);
        return Ok(OperationResult.Success("Purchase recorded."));
    }
}

[ApiController, Authorize, Route("api/orders")]
public sealed class OrdersWriteController(ICommerceService service, IUserContextService users) : ControllerBase
{
    [HttpPost] public ActionResult<OperationResult> Create(CreateOrderRequest request)
    {
        service.RecordOrder(users.GetRequiredUserId(User), request);
        return Ok(OperationResult.Success("Order recorded."));
    }
}

[ApiController, Authorize, Route("api/content")]
public sealed class ContentController(IPortalRepository repository) : ControllerBase
{
    [HttpGet("{type}")] public ActionResult<PaginatedDto<PortalContentDto>> Get(string type, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(Pagination.Page(repository.GetContent(type), page, pageSize));

    [HttpPost] public ActionResult<OperationResult> Create(CreateContentRequest request)
    {
        repository.AddContent(request);
        return Ok(OperationResult.Success("Content published."));
    }
}