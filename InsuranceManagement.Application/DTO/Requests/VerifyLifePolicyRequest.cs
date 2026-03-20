using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class VerifyLifePolicyRequest
    {
        public string SecretKey { get; set; }
        public string BeneficiaryEmail { get; set; }
    }

}
