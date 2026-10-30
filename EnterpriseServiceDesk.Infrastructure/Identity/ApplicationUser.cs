using EnterpriseServiceDesk.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseServiceDesk.Infrastructure.Identity
{
    public sealed class ApplicationUser : IdentityUser<Guid>
    {
        public Guid EmployeeId { get; set; }

        public Employee Employee { get; set; } = null!;
    }
}
