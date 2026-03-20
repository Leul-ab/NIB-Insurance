using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class PaymentTransactionResponse
    {
        public Guid ApplicationId { get; set; }
        public string Reference { get; set; }
        public string CheckoutUrl { get; set; }

        public Guid ClientId { get; set; }
        public string ClientFullName { get; set; }

        public decimal PremiumAmount { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsPaid { get; set; }

        public string InsuranceType { get; set; }


        public DateTime CreatedAt { get; set; }
    }
}
