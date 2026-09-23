using Microsoft.AspNetCore.Identity;
using VitalityPortal.Data;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Payments;
using VitalityPortal.Repositories;

namespace VitalityPortal.Services;

public interface IGoogleAuthService
{
    Task<(OperationResult Result, string? UserId)> AuthenticateGoogleUserAsync(GoogleLoginRequest request);
}

public sealed class GoogleAuthService(
    UserManager<ApplicationUser> userManager,
    IMemberRepository memberRepository,
    IPortalRepository portalRepository,
    IEmailSenderService emailService,
    ILogger<GoogleAuthService> logger) : IGoogleAuthService
{
    public async Task<(OperationResult Result, string? UserId)> AuthenticateGoogleUserAsync(GoogleLoginRequest request)
    {
        // Decode Google Identity Services ID Token payload if provided
        if (!string.IsNullOrWhiteSpace(request.IdToken) && string.IsNullOrWhiteSpace(request.Email))
        {
            try
            {
                var parts = request.IdToken.Split('.');
                if (parts.Length >= 2)
                {
                    var payloadJson = System.Text.Encoding.UTF8.GetString(Microsoft.IdentityModel.Tokens.Base64UrlEncoder.DecodeBytes(parts[1]));
                    using var doc = System.Text.Json.JsonDocument.Parse(payloadJson);
                    if (doc.RootElement.TryGetProperty("email", out var emailProp))
                    {
                        request.Email = emailProp.GetString();
                    }
                    if (doc.RootElement.TryGetProperty("name", out var nameProp))
                    {
                        request.Name = nameProp.GetString();
                    }
                    if (doc.RootElement.TryGetProperty("sub", out var subProp))
                    {
                        request.GoogleId = subProp.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to parse Google ID Token payload");
            }
        }

        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email))
        {
            return (OperationResult.Failure("Email is required for Google sign-in."), null);
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            return (OperationResult.Success("Google login successful."), user.UserName);
        }

        // Auto-provision new partner account from Google profile
        var baseUsername = email.Split('@')[0].Replace(".", "").Replace("-", "").ToLowerInvariant();
        var username = baseUsername;
        int counter = 1;
        while (await userManager.FindByNameAsync(username) is not null)
        {
            username = $"{baseUsername}{counter++}";
        }

        var nameParts = (request.Name ?? "Google Partner").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : "Google";
        var lastName = nameParts.Length > 1 ? nameParts[1] : "Partner";

        var newUser = new ApplicationUser
        {
            UserName = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Country = "South Africa",
            PhoneNumber = "0000000000",
            Rank = "Newbie",
            IsActivated = false
        };

        // Create with auto-generated secure password
        var randomPassword = $"Gg!{Guid.NewGuid():N}[12]";
        var createResult = await userManager.CreateAsync(newUser, randomPassword);
        if (!createResult.Succeeded)
        {
            var err = string.Join(" ", createResult.Errors.Select(e => e.Description));
            logger.LogError("Failed to create user via Google OAuth: {Err}", err);
            return (OperationResult.Failure(err), null);
        }

        // Register referral hierarchy under top sponsor
        memberRepository.Add(new Models.Members.CreateMemberRequest
        {
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Country = "South Africa",
            Mobile = "0000000000",
            Password = randomPassword,
            ConfirmPassword = randomPassword
        });

        portalRepository.AddReferral(new Models.Portal.ReferralDto(
            username,
            $"{firstName} {lastName}",
            "0000000000",
            "bhekaragnar",
            "Newbie",
            0m,
            "Pending",
            DateOnly.FromDateTime(DateTime.UtcNow)
        ));

        // Send welcome email
        _ = emailService.SendEmailAsync(
            email,
            "Welcome to ScaleEngine!",
            $"<h3>Hi {firstName},</h3><p>Welcome to <strong>ScaleEngine</strong>! Your account <code>{username}</code> is ready.</p><p>You can now sign in using Google anytime.</p>"
        );

        return (OperationResult.Success("Google registration successful. Welcome to ScaleEngine!"), username);
    }
}
