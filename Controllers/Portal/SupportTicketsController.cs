using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Data;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Portal;
using VitalityPortal.Repositories;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Authorize]
[Route("api/support-tickets")]
public sealed class SupportTicketsController(
    IPortalRepository portalRepository,
    IUserContextService userContextService,
    UserManager<ApplicationUser> userManager,
    IAiAssistantService aiAssistantService) : ControllerBase
{
    private async Task<string> GetUserDisplayNameAsync(string userId)
    {
        var user = await userManager.FindByNameAsync(userId);
        if (user is not null)
        {
            var full = $"{user.FirstName} {user.LastName}".Trim();
            if (!string.IsNullOrWhiteSpace(full)) return full;
        }
        return userId;
    }

    [HttpGet]
    public ActionResult<IReadOnlyList<SupportTicketDto>> GetTickets()
    {
        var userId = userContextService.GetRequiredUserId(User);
        return Ok(portalRepository.GetSupportTickets(userId));
    }

    [HttpGet("{id:int}")]
    public ActionResult<SupportTicketDetailDto> GetTicket(int id)
    {
        var userId = userContextService.GetRequiredUserId(User);
        var details = portalRepository.GetSupportTicketDetails(userId, id);
        return details is null ? NotFound(OperationResult.Failure("Ticket not found.")) : Ok(details);
    }

    [HttpPost]
    public async Task<ActionResult<OperationResult>> CreateTicket([FromBody] CreateTicketRequest request)
    {
        var userId = userContextService.GetRequiredUserId(User);
        var displayName = await GetUserDisplayNameAsync(userId);
        var result = portalRepository.CreateSupportTicket(userId, displayName, request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id:int}/reply")]
    public async Task<ActionResult<OperationResult>> ReplyTicket(int id, [FromBody] ReplyTicketRequest request)
    {
        var userId = userContextService.GetRequiredUserId(User);
        var displayName = await GetUserDisplayNameAsync(userId);
        var result = portalRepository.ReplySupportTicket(userId, displayName, id, request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    public sealed record AiDiagnoseTicketRequest(string? Note);

    [HttpPost("{id:int}/ai-diagnose")]
    public async Task<ActionResult<AiChatResponse>> AiDiagnoseTicket(int id, [FromBody] AiDiagnoseTicketRequest? request)
    {
        var userId = userContextService.GetRequiredUserId(User);
        var ticket = portalRepository.GetSupportTicketDetails(userId, id);
        if (ticket is null) return NotFound(OperationResult.Failure("Ticket not found."));

        var response = await aiAssistantService.DiagnoseTicketAsync(userId, ticket, request?.Note);
        return Ok(response);
    }

    [HttpPut("{id:int}/close")]
    public ActionResult<OperationResult> CloseTicket(int id)
    {
        var userId = userContextService.GetRequiredUserId(User);
        var result = portalRepository.CloseSupportTicket(userId, id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
