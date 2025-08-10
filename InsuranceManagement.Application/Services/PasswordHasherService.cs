

using InsuranceManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace InsuranceManagement.Application.Services { 

    public class PasswordHasherService
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string HashPassword(User user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        public bool VerifyPassword(User user, string password)
        {
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
