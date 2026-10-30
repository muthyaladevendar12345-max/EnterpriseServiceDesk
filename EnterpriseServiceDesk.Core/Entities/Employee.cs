using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseServiceDesk.Core.Entities
{
    public sealed class Employee
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string EmployeeNumber { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
