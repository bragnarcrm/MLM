using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Wallet;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Authorize]
[Route("api/wallet/withdrawals")]
public sealed class WalletController(IWalletService walletService, IUserContextService users) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OperationResult>> Create(CreateWithdrawalRequest request)
    {
        var result = await walletService.RequestWithdrawalAsync(users.GetRequiredUserId(User), request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}