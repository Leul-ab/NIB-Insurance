using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class ReviewedClaimDto
    {
        public Guid ClaimId { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string OperatorName { get; set; }
        public DateTime IncidentDate { get; set; }
        public string Location { get; set; }
        public string IncidentType { get; set; }
        public decimal ProposedAmount { get; set; }
        public DateTime ReviewedAt { get; set; }
        public Guid ReviewedByOperatorId { get; set; }
    }

}
