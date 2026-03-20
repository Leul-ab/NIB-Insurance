using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class ApproveClaimResponse
    {
        public Guid ClaimId { get; set; }
        public string ClientFullName { get; set; }
        public Guid ApprovedByOperatorId { get; set; }

        public ClaimStatus Status { get; set; }
        public decimal ApprovedAmount { get; set; }
        public DateTime ApprovedAt { get; set; }
    }

}
