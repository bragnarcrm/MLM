using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Portal;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController, Authorize, Route("api/payout-settings")]
public sealed class PayoutSettingsController(IWalletActivityService service) : ControllerBase
{
    [HttpGet] public ActionResult<PayoutSettingsDto> Get() => Ok(service.GetPayoutSettings());
    [HttpPut] public ActionResult<OperationResult> Save(PayoutSettingsRequest request) => Ok(service.SavePayoutSettings(request));
}

[ApiController, Authorize, Route("api/shopping")]
public sealed class ShoppingController(ICommerceService service, IUserContextService users) : ControllerBase
{
    [HttpGet("purchases")] public ActionResult<PaginatedDto<PurchaseDto>> Purchases([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetPurchases(users.GetRequiredUserId(User), page, pageSize));
}

[ApiController, Authorize, Route("api/orders")]
public sealed class OrdersController(ICommerceService service, IUserContextService users) : ControllerBase
{
    [HttpGet] public ActionResult<PaginatedDto<OrderDto>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetOrders(users.GetRequiredUserId(User), page, pageSize));
}

[ApiController, Authorize, Route("api/messages")]
public sealed class MessagesController(ICommunicationService service, IUserContextService users) : ControllerBase
{
    [HttpGet("inbox")] public ActionResult<PaginatedDto<PortalMessageDto>> Inbox([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetInbox(users.GetRequiredUserId(User), page, pageSize));
    [HttpGet("sent")] public ActionResult<PaginatedDto<PortalMessageDto>> Sent([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(service.GetSent(users.GetRequiredUserId(User), page, pageSize));
    [HttpPost] public ActionResult<OperationResult> Send(SendMessageRequest request) => Ok(service.Send(users.GetRequiredUserId(User), users.GetRequiredUserId(User), request));
    [HttpPut("{id:int}/read")] public ActionResult<OperationResult> MarkAsRead(int id) => Ok(service.MarkAsRead(users.GetRequiredUserId(User), id));
    [HttpDelete("{id:int}")] public ActionResult<OperationResult> Delete(int id) => Ok(service.Delete(users.GetRequiredUserId(User), id));
}

[ApiController, Authorize, Route("api/meetings")]
public sealed class MeetingsController(ICommunicationService service) : ControllerBase
{
    [HttpGet] public ActionResult<IReadOnlyList<ZoomMeetingDto>> Get() => Ok(service.GetMeetings());
}