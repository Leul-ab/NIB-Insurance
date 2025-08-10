

using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Domain.Enums;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Infrastructure.Seed
{
    public class AdminSeeder
    {
        public static async Task SeedAdminAsync(InsuranceDbContext context, string username, string email, string password)
        {
            if (!await context.Users.AnyAsync(u => u.Role == Domain.Enums.UserRole.Admin))
            {
                var admin = new User
                {
                    UserName = username,
                    Email = email,
                    Role = UserRole.Admin,
                    
                };

                var hasher = new PasswordHasher<User>();
                admin.PasswordHash = hasher.HashPassword(admin, password);

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
