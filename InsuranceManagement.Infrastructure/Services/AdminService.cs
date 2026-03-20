using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InsuranceManagement.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly InsuranceDbContext _context;

        public AdminService(InsuranceDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardStatsResponse> GetDashboardStatsAsync()
        {
            // Run each query sequentially to avoid DbContext concurrency issues
            var totalClients = await _context.Clients.CountAsync();
            var totalFinance = await _context.Users.CountAsync(u => u.Role == Domain.Enums.UserRole.Finance);
            var totalManagers = await _context.Users.CountAsync(u => u.Role == Domain.Enums.UserRole.Manager);
            var totalOperators = await _context.Users.CountAsync(u => u.Role == Domain.Enums.UserRole.Operator);

            return new DashboardStatsResponse
            {
                TotalClients = totalClients,
                TotalFinanceStaff = totalFinance,
                TotalManagers = totalManagers,
                TotalOperators = totalOperators
            };
        }

        public async Task<UserTrendsResponse> GetUserTrendsAsync()
        {
            var now = DateTime.UtcNow;

            var startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfYear = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // ✅ CLIENTS
            var clientThisWeek = await _context.Clients.CountAsync(c => c.CreatedAt >= startOfWeek);
            var clientThisMonth = await _context.Clients.CountAsync(c => c.CreatedAt >= startOfMonth);
            var clientThisYear = await _context.Clients.CountAsync(c => c.CreatedAt >= startOfYear);

            // ✅ STAFF (Finance, Manager, Operator)
            var staffRoles = new[]
            {
                Domain.Enums.UserRole.Finance,
                Domain.Enums.UserRole.Manager,
                Domain.Enums.UserRole.Operator
            };

            var staffThisWeek = await _context.Users.CountAsync(u => staffRoles.Contains(u.Role) && u.CreatedAt >= startOfWeek);
            var staffThisMonth = await _context.Users.CountAsync(u => staffRoles.Contains(u.Role) && u.CreatedAt >= startOfMonth);
            var staffThisYear = await _context.Users.CountAsync(u => staffRoles.Contains(u.Role) && u.CreatedAt >= startOfYear);

            return new UserTrendsResponse
            {
                Clients = new UserGroupTrends
                {
                    AddedThisWeek = clientThisWeek,
                    AddedThisMonth = clientThisMonth,
                    AddedThisYear = clientThisYear
                },
                Staff = new UserGroupTrends
                {
                    AddedThisWeek = staffThisWeek,
                    AddedThisMonth = staffThisMonth,
                    AddedThisYear = staffThisYear
                }
            };
        }
    }
}
