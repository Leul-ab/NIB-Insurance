using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class UserGroupTrends
    {
        public int AddedThisWeek { get; set; }
        public int AddedThisMonth { get; set; }
        public int AddedThisYear { get; set; }
    }
}
