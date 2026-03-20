using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class ClientUpdateRequest
    {
        public string FirstName { get; set; }
        public string FatherName { get; set; }
        public string GrandFatherName { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Region { get; set; }
        public string City { get; set; }
        public string SubCity { get; set; }

        public string NationalIdOrPassport { get; set; }
        public IFormFile PassportOrNationalIdImage { get; set; }

    }
}
