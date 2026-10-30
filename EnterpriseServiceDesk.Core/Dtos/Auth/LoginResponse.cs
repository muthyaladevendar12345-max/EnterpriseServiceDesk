using System;
using System.Collections.Generic;
using System.Text;

namespace EnterpriseServiceDesk.Core.Dtos.Auth
{
    public sealed record LoginResponse( string AccessToken, DateTimeOffset ExpiresAt, Guid EmployeeId, string FullName,string Email,string[] Roles);
}
