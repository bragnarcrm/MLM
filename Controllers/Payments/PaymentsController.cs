using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VitalityPortal.Models.Payments;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(
    IPaymentGatewayService paymentGatewayService,
    IUserContextService userContextService,
    IOptions<PayFastOptions> payFastOptions,
    IOptions<GoogleAuthOptions> googleOptions) : ControllerBase
{
    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        return Ok(new
        {
            PayFastMerchantId = payFastOptions.Value.MerchantId,
            IsPayFastSandbox = payFastOptions.Value.IsSandbox,
            GoogleClientId = googleOptions.Value.ClientId
        });
    }

    [Authorize]
    [HttpPost("payfast/initiate")]
    public ActionResult<PayFastInitiateResponseDto> InitiatePayFast([FromBody] InitiatePayFastRequest request)
    {
        var userId = userContextService.GetRequiredUserId(User);
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result = paymentGatewayService.InitiatePayFast(userId, $"{userId}@scaleengine.io", request, baseUrl);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("direct")]
    public async Task<ActionResult<PaymentResultDto>> ProcessDirectPayment([FromBody] DirectPaymentRequest request)
    {
        var userId = userContextService.GetRequiredUserId(User);
        var result = await paymentGatewayService.ProcessDirectPaymentAsync(userId, request);
        return Ok(result);
    }

    [HttpPost("payfast/notify")]
    public async Task<IActionResult> PayFastNotify([FromForm] IFormCollection form)
    {
        var success = await paymentGatewayService.HandlePayFastNotifyAsync(form);
        return success ? Ok() : BadRequest();
    }
}
