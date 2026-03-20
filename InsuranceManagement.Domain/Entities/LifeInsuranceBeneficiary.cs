using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class LifeInsuranceBeneficiary
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LifeInsuranceApplicationId { get; set; }

        public string Name { get; set; }
        public BeneficiaryRelation Relation { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? NationalIdFilePath { get; set; }
    }
}
