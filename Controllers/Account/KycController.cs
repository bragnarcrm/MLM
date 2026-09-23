using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Portal;
using VitalityPortal.Repositories;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Authorize]
[Route("api/kyc")]
public sealed class KycController(IPortalRepository portalRepository, IUserContextService userContextService) : ControllerBase
{
    [HttpGet("status")]
    public ActionResult<KycStatusDto> GetStatus()
    {
        var userId = userContextService.GetRequiredUserId(User);
        return Ok(portalRepository.GetKycStatus(userId));
    }

    [HttpPost("submit")]
    public ActionResult<OperationResult> Submit([FromBody] SubmitKycRequest request)
    {
        var userId = userContextService.GetRequiredUserId(User);
        var result = portalRepository.SubmitKyc(userId, request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    // Immediate self-approval helper for demo & instant testing verification
    [HttpPost("verify-demo")]
    public ActionResult<OperationResult> VerifyDemo()
    {
        var userId = userContextService.GetRequiredUserId(User);
        portalRepository.SetKycStatus(userId, "Verified", "Self-verified for testing & partner onboarding.");
        return Ok(OperationResult.Success("KYC verified successfully. Payout withdrawals unlocked."));
    }
}
