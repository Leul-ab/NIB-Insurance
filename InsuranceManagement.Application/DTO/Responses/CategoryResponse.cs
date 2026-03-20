using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? FullInsurancePercentage { get; set; }
        public decimal? ThirdPartyPercentage { get; set; }
        public decimal? HalfLifePrice { get; set; }
        public decimal? FullLifePrice { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ParentId { get; set; }
      


        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
