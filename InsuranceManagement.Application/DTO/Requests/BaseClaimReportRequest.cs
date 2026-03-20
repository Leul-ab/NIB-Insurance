using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public abstract class BaseClaimReportRequest
    {
        public DateTime IncidentDate { get; set; }
        public string Description { get; set; }
    }

}
