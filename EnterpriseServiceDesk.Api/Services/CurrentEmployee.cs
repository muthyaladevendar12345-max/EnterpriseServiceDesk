using EnterpriseServiceDesk.Core.Interfaces;
using System.Security.Claims;

namespace EnterpriseServiceDesk.Api.Services
{
    public sealed class CurrentEmployee : ICurrentEmployee
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentEmployee(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? EmployeeId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                return Guid.TryParse( user.FindFirstValue("employee_id"), out var employeeId)? employeeId:null;
            }
        }
    }
}
