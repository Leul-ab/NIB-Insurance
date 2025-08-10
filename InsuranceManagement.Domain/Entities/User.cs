using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public Operator? Operator { get; set; }
        public Client? Client { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public string? PasswordResetOtp { get; set; }
        public DateTime? OtpGeneratedAt { get; set; }
        public bool IsOtpVerified { get; set; } = false;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
    }
}
