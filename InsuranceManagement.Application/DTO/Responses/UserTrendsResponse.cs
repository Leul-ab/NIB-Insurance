using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class UserTrendsResponse
    {
        public UserGroupTrends Clients { get; set; } = new();
        public UserGroupTrends Staff { get; set; } = new();
    }
}
