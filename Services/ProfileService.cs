using Microsoft.AspNetCore.Identity;
using VitalityPortal.Data;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Profile;

namespace VitalityPortal.Services;

public sealed class ProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    public async Task<UserProfile> GetAsync(string userId)
    {
        var user = await userManager.FindByNameAsync(userId) ?? throw new UnauthorizedAccessException();
        return new UserProfile { Username = user.UserName ?? userId, FirstName = user.FirstName, LastName = user.LastName, Email = user.Email ?? string.Empty, Country = user.Country, Mobile = user.PhoneNumber ?? string.Empty, Rank = user.Rank ?? "Newbie" };
    }

    public async Task<OperationResult> UpdateAsync(string userId, UpdateProfileRequest request)
    {
        var user = await userManager.FindByNameAsync(userId);
        if (user is null) return OperationResult.Failure("Account not found.");
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Country = request.Country;
        user.PhoneNumber = request.Mobile;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded) return OperationResult.Failure(string.Join(" ", result.Errors.Select(error => error.Description)));
        return OperationResult.Success("Profile changes are ready to save.");
    }
}