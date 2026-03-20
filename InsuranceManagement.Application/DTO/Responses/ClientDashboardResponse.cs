using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class ClientDashboardResponse
    {
        public int TotalApplications { get; set; }
        public int ApprovedApplications { get; set; }

        public int MotorPolicies { get; set; }
        public int LifePolicies { get; set; }

        public decimal TotalPaidAmount { get; set; }
    }
}

