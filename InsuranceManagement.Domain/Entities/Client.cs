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

        public string LogoImageUrl { get; set; } = string.Empty;
        public string FullName { get; set; } = default!;
        public string MobliePhone { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

        public Guid? UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
