using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class MotorInsuranceImage
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public MotorInsuranceApplication Application { get; set; }

        public string ImageUrl { get; set; } = default!;
    }

}
