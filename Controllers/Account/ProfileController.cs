using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Profile;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public sealed class ProfileController(IProfileService profileService, IUserContextService users) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<object>> Get() => Ok(await profileService.GetAsync(users.GetRequiredUserId(User)));

    [HttpPut]
    public async Task<ActionResult<OperationResult>> Update(UpdateProfileRequest request) => Ok(await profileService.UpdateAsync(users.GetRequiredUserId(User), request));
}