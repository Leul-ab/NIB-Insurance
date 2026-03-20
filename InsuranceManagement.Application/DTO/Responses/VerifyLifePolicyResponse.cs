using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class VerifyLifePolicyResponse
    {
        public Guid ApplicationId { get; set; }
        public string ClientFullName { get; set; }
        public string ClientEmail { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

}
