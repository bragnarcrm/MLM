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
[Route("api/team-chat")]
public sealed class TeamChatController(
    IPortalRepository repository,
    IUserContextService users,
    UserManager<ApplicationUser> userManager) : ControllerBase
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

    [HttpGet("contacts")]
    public ActionResult<IReadOnlyList<TeamChatContactDto>> GetContacts()
    {
        var userId = users.GetRequiredUserId(User);
        return Ok(repository.GetChatContacts(userId));
    }

    [HttpGet("thread/{otherUserId}")]
    public ActionResult<IReadOnlyList<TeamChatMessageDto>> GetThread(string otherUserId)
    {
        var userId = users.GetRequiredUserId(User);
        return Ok(repository.GetChatThread(userId, otherUserId));
    }

    [HttpPost("send")]
    public async Task<ActionResult<OperationResult>> SendMessage([FromBody] SendTeamChatRequest request)
    {
        var userId = users.GetRequiredUserId(User);
        var senderName = await GetUserDisplayNameAsync(userId);
        var result = repository.SendChatMessage(userId, senderName, request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
