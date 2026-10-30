using EnterpriseServiceDesk.Core.Dtos;

namespace EnterpriseServiceDesk.Mvc.Models
{
    public sealed class LeaveBalancesViewModel
    {
        public int Year { get; set; }

        public IReadOnlyList<LeaveBalanceDto> Balances { get; set; } = Array.Empty<LeaveBalanceDto>();

        public string? ErrorMessage { get; set; }
    }
}
