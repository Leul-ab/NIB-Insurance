using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class MotorInsuranceApplicationResponse
    {
        public Guid ApplicationId { get; set; }
        public Guid ClientId { get; set; }

        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }

        public string Model { get; set; }
        public string PlateNumber { get; set; }
        public int YearOfManufacture { get; set; }
        public string EngineNumber { get; set; }
        public string ChassisNumber { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal CalculatedPremium { get; set; }
        public InsuranceType InsuranceType { get; set; }

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

        public string CarImageUrl { get; set; }
        public string CarLibreImageUrl { get; set; }
    }



}
