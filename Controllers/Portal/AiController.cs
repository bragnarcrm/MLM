using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Authorize]
[Route("api/ai")]
public sealed class AiController(IAiAssistantService aiService, IUserContextService users) : ControllerBase
{
    [HttpPost("chat")]
    public async Task<ActionResult<AiChatResponse>> Chat([FromBody] AiChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { message = "Message content is required." });
        }

        var userId = users.GetRequiredUserId(User);
        var response = await aiService.AskAsync(userId, request);
        return Ok(response);
    }

    [HttpGet("context")]
    public async Task<ActionResult> GetContext()
    {
        var userId = users.GetRequiredUserId(User);
        var summary = await aiService.GetUserContextSummaryAsync(userId);
        return Ok(new { summary });
    }
}
