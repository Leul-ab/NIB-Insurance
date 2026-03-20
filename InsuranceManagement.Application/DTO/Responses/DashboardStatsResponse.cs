using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class DashboardStatsResponse
    {
        public int TotalClients { get; set; }
        public int TotalFinanceStaff { get; set; }
        public int TotalManagers { get; set; }
        public int TotalOperators { get; set; }
        public int TotalStaff => TotalFinanceStaff + TotalManagers + TotalOperators;
    }
}
