using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class Manager
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string FatherName { get; set; }
        public string GrandFatherName { get; set; }
      

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }
        public string NationalIdOrPassport { get; set; }
        public Gender? Gender { get; set; }

        public string Region { get; set; }
        public string City { get; set; }
        public string SubCity { get; set; }

        public string? LogoImageUrl { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
