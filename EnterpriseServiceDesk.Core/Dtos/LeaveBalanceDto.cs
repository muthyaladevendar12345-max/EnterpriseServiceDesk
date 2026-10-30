using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseServiceDesk.Core.Dtos
{
    public sealed record LeaveBalanceDto(
     int Year,
     string LeaveType,
     decimal AllocatedDays,
     decimal UsedDays,
     decimal RemainingDays);
}
