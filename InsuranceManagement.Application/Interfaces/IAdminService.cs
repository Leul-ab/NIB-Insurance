using InsuranceManagement.Application.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.Interfaces
{
    public interface IAdminService
    {
        Task<DashboardStatsResponse> GetDashboardStatsAsync();
        Task<UserTrendsResponse> GetUserTrendsAsync();

    }
}
