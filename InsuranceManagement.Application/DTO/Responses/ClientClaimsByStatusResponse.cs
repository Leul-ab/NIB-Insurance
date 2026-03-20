using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class ClientClaimsByStatusResponse
    {
        public List<ClaimResponse> PaidClaims { get; set; } = new();
        public List<ClaimResponse> PendingClaims { get; set; } = new();
        public List<ClaimResponse> RejectedClaims { get; set; } = new();
    }

}
