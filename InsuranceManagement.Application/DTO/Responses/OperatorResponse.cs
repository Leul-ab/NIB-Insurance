using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class OperatorResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> CategoryNames { get; set; } = new List<string>();
    }
}
