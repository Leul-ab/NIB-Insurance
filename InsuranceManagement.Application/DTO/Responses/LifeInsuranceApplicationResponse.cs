using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class LifeInsuranceApplicationResponse
    {
        public Guid ApplicationId { get; set; }
        public Guid ClientId { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }

        public float Age { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public decimal LifePrice { get; set; }
        public LifeInsuranceType LifeInsuranceType { get; set; }

        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Message { get; set; }

        // ===== Client Personal Info =====
        public string ClientFullName { get; set; }      // First + Father + GrandFather
        public string ClientEmail { get; set; }
        public string ClientPhoneNumber { get; set; }
        public Gender ClientGender { get; set; }
        public DateTime ClientDateOfBirth { get; set; }
        public string ClientNationalIdOrPassport { get; set; }
        public string ClientPassportOrNationalIdImageUrl { get; set; }

        // NEW BENEFICIARY FIELDS
        //public string BeneficiaryName { get; set; }
        //public BeneficiaryRelation BeneficiaryRelation { get; set; }
        //public string BeneficiaryPhoneNumber { get; set; }
        //public string BeneficiaryNationalIdPath { get; set; }
        //public string? BeneficiaryEmail { get; set; }

        public List<BeneficiaryResponse> Beneficiaries { get; set; } = new();
        public string? SecretKey { get; set; }


        //public string CarImageUrl { get; set; }
        //public string CarLibreImageUrl { get; set; }
    }
}
