using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class RevealSecretKeyRequest
    {
        public Guid ApplicationId { get; set; }
        public string Password { get; set; }
    }

}
