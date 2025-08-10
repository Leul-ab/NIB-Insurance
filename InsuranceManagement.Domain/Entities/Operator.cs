using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class Operator
    {
        public Guid Id { get; set; } = Guid.NewGuid();


        //Personal info
        public string LogoImageUrl { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = false;


        //location
        public string Region { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string SubCity { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }



        public DateTime CreatedAt { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public ICollection<OperatorCategory> Categories { get; set; } = new List<OperatorCategory>();
    }
}
