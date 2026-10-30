using EnterpriseServiceDesk.Core.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseServiceDesk.Core.Interfaces
{
    public interface ILeaveBalanceService
    {
        // Null means the current employee is missing or inactive.
        Task<IReadOnlyList<LeaveBalanceDto>?> GetMyBalancesAsync(int year,CancellationToken cancellationToken = default);
    }
}
