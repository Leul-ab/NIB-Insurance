using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class RevealSecretKeyResponse
    {
        public Guid ApplicationId { get; set; }
        public string SecretKey { get; set; }
    }

}
