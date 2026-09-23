using VitalityPortal.Models.Common;
using VitalityPortal.Models.Profile;

namespace VitalityPortal.Services;

public interface IProfileService
{
    Task<UserProfile> GetAsync(string userId);
    Task<OperationResult> UpdateAsync(string userId, UpdateProfileRequest request);
}