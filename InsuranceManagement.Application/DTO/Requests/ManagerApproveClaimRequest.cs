using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class ManagerApproveClaimRequest
    {
        public bool Approve { get; set; }   // true = approve, false = reject
    }

}
