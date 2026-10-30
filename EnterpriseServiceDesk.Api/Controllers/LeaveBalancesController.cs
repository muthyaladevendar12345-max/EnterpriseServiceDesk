using System.ComponentModel.DataAnnotations;
using EnterpriseServiceDesk.Core.Dtos;
using EnterpriseServiceDesk.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseServiceDesk.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/leave-balances")]
public sealed class LeaveBalancesController : ControllerBase
{
    private readonly ILeaveBalanceService _leaveBalanceService;

    public LeaveBalancesController(ILeaveBalanceService leaveBalanceService)
    {
        _leaveBalanceService = leaveBalanceService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<IReadOnlyList<LeaveBalanceDto>>> GetMine([FromQuery, Range(2000, 9999)] int? year,
        CancellationToken cancellationToken)
    {
        var result = await _leaveBalanceService.GetMyBalancesAsync(year ?? DateTime.UtcNow.Year,cancellationToken);

        if (result is null)
        {
            return Unauthorized();
        }

        return Ok(result);
    }
}