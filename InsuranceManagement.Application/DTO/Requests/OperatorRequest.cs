using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class OperatorRequest
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public List<Guid> CategoryIds { get; set; } = new List<Guid>();
    }
}
