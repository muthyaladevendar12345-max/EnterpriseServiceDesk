using EnterpriseServiceDesk.Core.Dtos;
using EnterpriseServiceDesk.Core.Interfaces;
using EnterpriseServiceDesk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseServiceDesk.Infrastructure.Services;

public sealed class LeaveBalanceService : ILeaveBalanceService
{
    private readonly ServiceDeskDbContext _db;
    private readonly ICurrentEmployee _currentEmployee;

    public LeaveBalanceService(ServiceDeskDbContext db,ICurrentEmployee currentEmployee)
    {
        _db = db;
        _currentEmployee = currentEmployee;
    }

    public async Task<IReadOnlyList<LeaveBalanceDto>?> GetMyBalancesAsync(int year,CancellationToken cancellationToken = default)
    {
        if (year is < 2000 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        var employeeId = _currentEmployee.EmployeeId;

        if (employeeId is null)
        {
            return null;
        }

        var isActive = await _db.Employees.AnyAsync(x => x.Id == employeeId.Value && x.IsActive,cancellationToken);

        if (!isActive)
        {
            return null;
        }

        return await _db.LeaveBalances.AsNoTracking().Where(x =>x.EmployeeId == employeeId.Value && x.Year == year)
            .OrderBy(x => x.LeaveType)
            .Select(x => new LeaveBalanceDto(
                x.Year,
                x.LeaveType,
                x.AllocatedDays,
                x.UsedDays,
                x.AllocatedDays - x.UsedDays))
            .ToListAsync(cancellationToken);
    }
}