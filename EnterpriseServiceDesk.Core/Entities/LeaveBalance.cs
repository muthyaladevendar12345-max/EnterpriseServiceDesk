using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseServiceDesk.Core.Entities
{
    public sealed class LeaveBalance
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;

        public int Year { get; set; }

        public string LeaveType { get; set; } = string.Empty;

        public decimal AllocatedDays { get; set; }

        public decimal UsedDays { get; set; }

        public decimal RemainingDays => AllocatedDays - UsedDays;
    }
}
