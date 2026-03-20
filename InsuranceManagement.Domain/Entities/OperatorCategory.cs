using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class OperatorCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? FullInsurancePercentage { get; set; }
        public decimal? ThirdPartyPercentage { get; set; }
        public decimal? HalfLifePrice { get; set; }
        public decimal? FullLifePrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // NEW — parent category (null = main category)
        public Guid? ParentId { get; set; }
        public OperatorCategory? Parent { get; set; }

        // NEW — child categories
        public List<OperatorCategory> SubCategories { get; set; } = new();

        // Already existing
        public List<Operator>? Operators { get; set; }
    }

}
