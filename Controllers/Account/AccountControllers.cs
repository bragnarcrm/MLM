using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Data;
using VitalityPortal.Models.Account;
using VitalityPortal.Models.Common;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController, AllowAnonymous, Route("api/registration")]
public sealed class RegistrationController(IRegistrationService service, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("availability")]
    public async Task<ActionResult<UsernameAvailabilityDto>> Availability([FromQuery] string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return BadRequest(new UsernameAvailabilityDto(false, "Enter a username."));
        var available = await userManager.FindByNameAsync(username) is null;
        return Ok(new UsernameAvailabilityDto(available, available ? "Username is available." : "Username is already in use."));
    }

    [HttpPost] public async Task<ActionResult<OperationResult>> Register(RegisterAccountRequest request)
    {
        var result = await service.RegisterAsync(request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}

[ApiController, Authorize, Route("api/credentials")]
public sealed class CredentialsController(ICredentialService service, IUserContextService users) : ControllerBase
{
    [HttpPut("password")] public async Task<ActionResult<OperationResult>> ChangePassword(ChangePasswordRequest request) => Result(await service.ChangePasswordAsync(users.GetRequiredUserId(User), request));
    [HttpPut("transaction-password")] public async Task<ActionResult<OperationResult>> SetTransactionPassword(ChangeTransactionPasswordRequest request) => Result(await service.SetTransactionPasswordAsync(users.GetRequiredUserId(User), request));
    [HttpPost("transaction-password/generate")] public async Task<ActionResult<OperationResult>> GenerateTransactionPassword() => Ok(await service.GenerateTransactionPasswordAsync(users.GetRequiredUserId(User)));
    [HttpGet("transaction-password/history")] public async Task<ActionResult<IReadOnlyList<TransactionPasswordLogDto>>> GetTransactionPasswordHistory() => Ok(await service.GetTransactionPasswordLogsAsync(users.GetRequiredUserId(User)));
    private ActionResult<OperationResult> Result(OperationResult result) => result.Succeeded ? Ok(result) : BadRequest(result);
}

[ApiController, Authorize, Route("api/activation")]
public sealed class ActivationController(IActivationService service, IUserContextService users) : ControllerBase
{
    [HttpGet] public ActionResult<AccountStatusDto> Status() => Ok(service.GetStatus(users.GetRequiredUserId(User)));
    [HttpPost] public ActionResult<OperationResult> Activate(ActivateAccountRequest request) => Ok(service.Activate(users.GetRequiredUserId(User), request));
}

[ApiController, AllowAnonymous, Route("api/account/forgot-password")]
public sealed class ForgotPasswordController(IPasswordResetService service) : ControllerBase
{
    [HttpPost] public async Task<ActionResult<OperationResult>> RequestReset(ForgotPasswordRequest request)
    {
        var result = await service.RequestResetAsync(request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}