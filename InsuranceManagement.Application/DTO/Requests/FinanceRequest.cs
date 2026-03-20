using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class FinanceRequest
    {
        public string FirstName { get; set; }
        public string FatherName { get; set; }
        public string GrandFatherName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public IFormFile? LogoImageUrl { get; set; }

        // Contact Info
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Address Info
        public string Region { get; set; }
        public string City { get; set; }
        public string SubCity { get; set; }

        // Identification
        public string NationalIdOrPassport { get; set; }
        public string Password { get; set; }

        // Automatically generated
        public DateTime CreatedAt { get; set; }
    }
}
