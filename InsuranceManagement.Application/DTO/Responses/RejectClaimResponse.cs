using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class RejectClaimResponse
    {
        public Guid ClaimId { get; set; }
        public Guid? RejectedByOperatorId { get; set; }

        public ClaimStatus Status { get; set; }
        public string? RejectReason { get; set; }
        public DateTime RejectedAt { get; set; }

        public Guid? RejectedByManagerId { get; set; }
        public DateTime? RejectedByManagerAt { get; set; }
        public string? RejectionReason { get; set; }

    }
}
