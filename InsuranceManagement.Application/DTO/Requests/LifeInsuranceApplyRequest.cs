using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class LifeInsuranceApplyRequest
    {
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }

        public float Age { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public string? Message { get; set; }
        public LifeInsuranceType LifeInsuranceType { get; set; }

        // NEW BENEFICIARY FIELDS
        //public string BeneficiaryName { get; set; }
        //public BeneficiaryRelation BeneficiaryRelation { get; set; }
        //public string BeneficiaryPhoneNumber { get; set; }
        //public IFormFile? BeneficiaryNationalIdFile { get; set; }
        //public string? BeneficiaryEmail { get; set; }
        //public string BeneficiariesJson { get; set; } = null!;
        //public List<BeneficiaryRequest> Beneficiaries { get; set; } = new();
        //public List<IFormFile>? NationalIdFiles { get; set; }



    }
}
