using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalityPortal.Models.Common;
using VitalityPortal.Models.Portal;
using VitalityPortal.Repositories;
using VitalityPortal.Services;

namespace VitalityPortal.Controllers;

[ApiController]
[Authorize]
[Route("api/autoship")]
public sealed class AutoShipController(IPortalRepository repository, IUserContextService users) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<AutoShipDto>> Get()
    {
        var userId = users.GetRequiredUserId(User);
        return Ok(repository.GetAutoShips(userId));
    }

    [HttpPost]
    public ActionResult<OperationResult> Create([FromBody] CreateAutoShipRequest request)
    {
        var userId = users.GetRequiredUserId(User);
        var result = repository.AddAutoShip(userId, request);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:int}/status")]
    public ActionResult<OperationResult> UpdateStatus(int id, [FromBody] UpdateAutoShipStatusRequest request)
    {
        var userId = users.GetRequiredUserId(User);
        var result = repository.UpdateAutoShipStatus(userId, id, request.Status);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<OperationResult> Delete(int id)
    {
        var userId = users.GetRequiredUserId(User);
        var result = repository.DeleteAutoShip(userId, id);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}
