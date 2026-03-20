using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class ManagerDashboardStatsDto
    {
        public int TotalClaims { get; set; }

        public int ReviewedByOperatorCount { get; set; }
        public int ApprovedByManagerCount { get; set; }
        public int RejectedByManagerCount { get; set; }

        public int LifeClaimsCount { get; set; }
        public int MotorClaimsCount { get; set; }

        public decimal TotalApprovedAmount { get; set; }
        public decimal ApprovedThisMonthAmount { get; set; }
        public decimal ApprovedThisYearAmount { get; set; }

        public int ReviewedTodayCount { get; set; }
    }

}
