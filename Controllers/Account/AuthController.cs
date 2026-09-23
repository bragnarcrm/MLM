using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Auth;
using VitalityPortal.Models.Payments;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAuthenticationService authenticationService,
    IGoogleAuthService googleAuthService,
    IJwtTokenService jwtTokenService,
    Microsoft.Extensions.Options.IOptions<GoogleAuthOptions> googleOptions) : ControllerBase
{
    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        return Ok(new
        {
            GoogleClientId = googleOptions.Value.ClientId
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequest request)
    {
        var result = await authenticationService.AuthenticateAsync(request);
        return result.Succeeded
            ? Ok(new LoginResponseDto(result.Message, jwtTokenService.CreateToken(request.UserId)))
            : BadRequest(new LoginResponseDto(result.Message, null));
    }

    [HttpPost("google")]
    public async Task<ActionResult<LoginResponseDto>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var (result, userId) = await googleAuthService.AuthenticateGoogleUserAsync(request);
        if (!result.Succeeded || string.IsNullOrEmpty(userId))
        {
            return BadRequest(new LoginResponseDto(result.Message, null));
        }

        return Ok(new LoginResponseDto(result.Message, jwtTokenService.CreateToken(userId)));
    }
}