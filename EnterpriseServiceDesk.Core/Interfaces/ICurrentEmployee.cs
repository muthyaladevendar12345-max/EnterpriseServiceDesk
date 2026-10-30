using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseServiceDesk.Core.Interfaces
{
    public interface ICurrentEmployee
    {
        Guid? EmployeeId { get; }
    }
}
