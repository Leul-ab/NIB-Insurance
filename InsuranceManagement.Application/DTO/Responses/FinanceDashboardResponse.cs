using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class FinanceDashboardResponse
    {
        public int TotalApplications { get; set; }

        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }

        public int MotorApplications { get; set; }
        public int LifeApplications { get; set; }
    }
}

