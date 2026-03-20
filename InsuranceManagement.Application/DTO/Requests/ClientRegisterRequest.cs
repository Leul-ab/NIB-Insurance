using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class ClientRegisterRequest
    {
        //from user
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string FatherName { get; set; }
        public string GrandFatherName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Region { get; set; }
        public string City { get; set; }
        public string SubCity { get; set; }

        public string Password { get; set; }

        public string NationalIdOrPassport { get; set; }
        public Gender? Gender { get; set; }

        public string? LogoImageUrl { get; set; }
        public IFormFile PassportOrNationalIdImage { get; set; }

    }
}
