using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class OperatorResponse
    {
        //from User
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;


        //personal info
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }


        //location
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string SubCity { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<string> CategoryNames { get; set; } = new List<string>();
    }
}
