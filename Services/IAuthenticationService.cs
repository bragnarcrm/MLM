using VitalityPortal.Models.Auth;
using VitalityPortal.Models.Common;

namespace VitalityPortal.Services;

public interface IAuthenticationService
{
    Task<OperationResult> AuthenticateAsync(LoginRequest request);
}