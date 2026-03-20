using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class LifeInsuranceApplication
    {
        public Guid Id { get; set; }

        // link to client
        public Guid ClientId { get; set; }
        public Client Client { get; set; }

        // selected category + subcategory
        public Guid CategoryId { get; set; }
        public OperatorCategory Category { get; set; }

        public Guid SubCategoryId { get; set; }
        public OperatorCategory SubCategory { get; set; }

        // Motor-specific data
        public float Age { get; set; }
        public float Height { get; set; } 
        public float Weight { get; set; }
        public string? Message { get; set; }
        // Calculated Insurance price (2.5%)
        public decimal LifePrice { get; set; }
        public LifeInsuranceType LifeInsuranceType { get; set; }

        // system
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Pending"; // approved/rejected later
        public string PaymentStatus { get; set; } = "Unpaid"; // paid/unpaid later

        //payment
        public string? PaymentReference { get; set; }
        public bool IsPaid { get; set; } = false;


        //Beneficiary
        //public string BeneficiaryName { get; set; }
        //public BeneficiaryRelation BeneficiaryRelation { get; set; }
        //public string BeneficiaryPhoneNumber { get; set; }
        //public string? BeneficiaryNationalIdPath { get; set; }
        //public string? BeneficiaryEmail { get; set; }

        public ICollection<LifeInsuranceBeneficiary> Beneficiaries { get; set; } = new List<LifeInsuranceBeneficiary>();
        public string? SecretKey { get; set; }

        //public string CarImagePath { get; set; }
        //public string CarLibreImagePath { get; set; }
    }
}
