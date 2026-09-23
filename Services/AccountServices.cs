using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using VitalityPortal.Data;
using VitalityPortal.Models.Account;
using VitalityPortal.Models.Common;
using VitalityPortal.Repositories;

namespace VitalityPortal.Services;

public interface IUserContextService { string GetRequiredUserId(ClaimsPrincipal user); }
public interface IRegistrationService { Task<OperationResult> RegisterAsync(RegisterAccountRequest request); }
public interface ICredentialService {
    Task<OperationResult> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    Task<OperationResult> SetTransactionPasswordAsync(string userId, ChangeTransactionPasswordRequest request);
    Task<OperationResult> GenerateTransactionPasswordAsync(string userId);
    Task<IReadOnlyList<TransactionPasswordLogDto>> GetTransactionPasswordLogsAsync(string userId);
}
public interface IActivationService { AccountStatusDto GetStatus(string userId); OperationResult Activate(string userId, ActivateAccountRequest request); }
public interface IPasswordResetService { Task<OperationResult> RequestResetAsync(ForgotPasswordRequest request); }

public sealed class UserContextService : IUserContextService
{
    public string GetRequiredUserId(ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
}

public sealed class RegistrationService(IMemberRepository memberRepository, IPortalRepository portalRepository, UserManager<ApplicationUser> userManager) : IRegistrationService
{
    public async Task<OperationResult> RegisterAsync(RegisterAccountRequest request)
    {
        if (request.Password != request.ConfirmPassword) return OperationResult.Failure("Passwords do not match.");
        var identityUser = new ApplicationUser { UserName = request.Username, Email = request.Email, FirstName = request.FirstName, LastName = request.LastName, Country = request.Country, PhoneNumber = request.Mobile };
        var identityResult = await userManager.CreateAsync(identityUser, request.Password);
        if (!identityResult.Succeeded) return OperationResult.Failure(string.Join(" ", identityResult.Errors.Select(error => error.Description)));
        memberRepository.Add(new Models.Members.CreateMemberRequest { Username = request.Username, FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, Country = request.Country, Mobile = request.Mobile, Password = request.Password, ConfirmPassword = request.ConfirmPassword });
        portalRepository.AddReferral(new Models.Portal.ReferralDto(request.Username, $"{request.FirstName} {request.LastName}", request.Mobile, request.SponsorId ?? "", "Newbie", 0m, "Pending", DateOnly.FromDateTime(DateTime.UtcNow)));
        return OperationResult.Success("Registration completed. You can now log in.");
    }
}

public sealed class CredentialService(UserManager<ApplicationUser> userManager, IPasswordHasher<ApplicationUser> passwordHasher, PortalDbContext context) : ICredentialService
{
    public async Task<OperationResult> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await userManager.FindByNameAsync(userId);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.OldPassword)) return OperationResult.Failure("Your old password is incorrect.");
        if (request.NewPassword != request.ConfirmPassword) return OperationResult.Failure("Passwords do not match.");
        var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        return result.Succeeded ? OperationResult.Success("Password changed successfully.") : OperationResult.Failure(string.Join(" ", result.Errors.Select(error => error.Description)));
    }

    public async Task<OperationResult> SetTransactionPasswordAsync(string userId, ChangeTransactionPasswordRequest request)
    {
        if (request.TransactionPassword != request.ConfirmPassword) return OperationResult.Failure("Transaction passwords do not match.");
        var user = await userManager.FindByNameAsync(userId);
        if (user is null) return OperationResult.Failure("Account not found.");
        user.TransactionPasswordHash = passwordHasher.HashPassword(user, request.TransactionPassword);
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            context.TransactionPasswordLogs.Add(new TransactionPasswordLogEntity
            {
                UserId = userId,
                Action = "Custom Transaction Password Updated",
                Remarks = "Transaction authorization password updated by partner",
                Status = "Success / Active",
                IpAddress = "127.0.0.1",
                OccurredAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            return OperationResult.Success("Transaction password saved successfully.");
        }
        return OperationResult.Failure("Transaction password could not be saved.");
    }

    public async Task<OperationResult> GenerateTransactionPasswordAsync(string userId)
    {
        var user = await userManager.FindByNameAsync(userId);
        if (user is null) return OperationResult.Failure("Account not found.");
        var newPin = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        user.TransactionPasswordHash = passwordHasher.HashPassword(user, newPin);
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            context.TransactionPasswordLogs.Add(new TransactionPasswordLogEntity
            {
                UserId = userId,
                Action = "Generated Transaction Password",
                Remarks = $"Temporary PIN [{newPin}] generated & dispatched to {user.Email}",
                Status = "Success / Active",
                IpAddress = "127.0.0.1",
                OccurredAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            return OperationResult.Success($"Transaction password generated successfully! Your temporary security PIN is: {newPin}. Dispatched to {user.Email}.");
        }
        return OperationResult.Failure("Transaction password could not be generated.");
    }

    public async Task<IReadOnlyList<TransactionPasswordLogDto>> GetTransactionPasswordLogsAsync(string userId)
    {
        var logs = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            context.TransactionPasswordLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.OccurredAt)
                .Select(l => new TransactionPasswordLogDto(l.Id, l.Action, l.Remarks, l.Status, l.IpAddress, l.OccurredAt))
        );

        if (logs.Count == 0)
        {
            var user = await userManager.FindByNameAsync(userId);
            var initialLog = new TransactionPasswordLogEntity
            {
                UserId = userId,
                Action = "Security Profile Initialization",
                Remarks = $"Partner security registered ({user?.Email ?? "system"})",
                Status = "Active / Verified",
                IpAddress = "127.0.0.1",
                OccurredAt = DateTime.UtcNow.AddDays(-1)
            };
            context.TransactionPasswordLogs.Add(initialLog);
            await context.SaveChangesAsync();
            logs.Add(new TransactionPasswordLogDto(initialLog.Id, initialLog.Action, initialLog.Remarks, initialLog.Status, initialLog.IpAddress, initialLog.OccurredAt));
        }

        return logs;
    }
}

public sealed class ActivationService(IAccountRepository accountRepository, Repositories.IPortalRepository portalRepository, ICommissionEngine commissionEngine) : IActivationService
{
    public AccountStatusDto GetStatus(string userId) => accountRepository.GetStatus(userId);
    public OperationResult Activate(string userId, ActivateAccountRequest request)
    {
        accountRepository.Activate(userId);
        portalRepository.SetReferralActive(userId);
        commissionEngine.ProcessPurchase(userId, request.Amount);
        return OperationResult.Success("Account activation payment recorded.");
    }
}

public sealed class PasswordResetService(UserManager<ApplicationUser> userManager) : IPasswordResetService
{
    public async Task<OperationResult> RequestResetAsync(ForgotPasswordRequest request)
    {
        var user = await userManager.FindByNameAsync(request.Username);
        if (user is null || !string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            return OperationResult.Failure("We couldn't find an account matching that username and e-mail.");

        var temporaryPassword = $"Reset-{Guid.NewGuid().ToString("N")[..8]}";
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, temporaryPassword);
        return result.Succeeded
            ? OperationResult.Success($"A temporary password has been issued: {temporaryPassword}. Sign in with it and change your password immediately.")
            : OperationResult.Failure(string.Join(" ", result.Errors.Select(error => error.Description)));
    }
}