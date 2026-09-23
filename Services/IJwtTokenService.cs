namespace VitalityPortal.Services;

public interface IJwtTokenService
{
    string CreateToken(string userId);
}