using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public Guid? UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
