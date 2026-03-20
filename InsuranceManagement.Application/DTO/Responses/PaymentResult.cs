using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string Reference { get; set; } = null!;
        public string? FailureReason { get; set; }
    }

}
