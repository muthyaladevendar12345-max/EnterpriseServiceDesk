using System.ComponentModel;
using EnterpriseServiceDesk.Core.Dtos;
using EnterpriseServiceDesk.Core.Interfaces;

namespace EnterpriseServiceDesk.Infrastructure.Tools;

public sealed class HrTools
{
    private readonly ILeaveBalanceService _leaveBalanceService;

    public HrTools(ILeaveBalanceService leaveBalanceService)
    {
        _leaveBalanceService = leaveBalanceService;
    }

    [Description(
        "Gets the authenticated employee's leave balances " +
        "for the current year. Returns allocated, used, and remaining days. " +
        "Cannot retrieve another employee's information.")]
    public async Task<IReadOnlyList<LeaveBalanceDto>>GetLeaveBalanceAsync(CancellationToken cancellationToken = default)
    {
        var balances = await _leaveBalanceService.GetMyBalancesAsync(DateTime.UtcNow.Year,cancellationToken);

        return balances?? throw new UnauthorizedAccessException("An active authenticated employee is required.");
    }
}