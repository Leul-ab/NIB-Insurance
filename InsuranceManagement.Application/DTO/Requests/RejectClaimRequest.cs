using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class RejectClaimRequest
    {
        public string RejectionReason { get; set; } = string.Empty;
    }


}
