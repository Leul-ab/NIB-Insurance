using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class PaymentInitializeResponse
    {
        public string CheckoutUrl { get; set; }
        public string Reference { get; set; }
    }
}
