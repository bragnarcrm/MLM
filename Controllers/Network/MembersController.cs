using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Members;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Authorize]
[Route("api/members")]
public sealed class MembersController(IMemberService memberService, IUserContextService users) : ControllerBase
{
    [HttpPost]
    public ActionResult<OperationResult> Create(CreateMemberRequest request)
    {
        var result = memberService.Register(users.GetRequiredUserId(User), request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}")]
    public ActionResult Get(int id)
    {
        var member = memberService.GetById(id);
        return member is null ? NotFound() : Ok(member);
    }

    [HttpPut("{id:int}")]
    public ActionResult<OperationResult> Update(int id, UpdateReferralRequest request)
    {
        var result = memberService.Update(id, request);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<OperationResult> Delete(int id)
    {
        var result = memberService.Delete(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }
}