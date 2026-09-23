using Microsoft.AspNetCore.Identity;
using VitalityPortal.Models.Auth;
using VitalityPortal.Models.Common;
using VitalityPortal.Data;

namespace VitalityPortal.Services;

public sealed class AuthenticationService(UserManager<ApplicationUser> userManager) : IAuthenticationService
{
    public async Task<OperationResult> AuthenticateAsync(LoginRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserId);
        return user is not null && await userManager.CheckPasswordAsync(user, request.Password)
        ? OperationResult.Success("Login successful. Opening dashboard...")
        : OperationResult.Failure("User ID or password is incorrect.");
    }
}