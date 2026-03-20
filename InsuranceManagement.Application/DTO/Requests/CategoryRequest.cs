using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class CategoryRequest
    {
        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public decimal FullInsurancePercentage { get; set; }
        public decimal ThirdPartyPercentage { get; set; }
        public decimal HalfLifePrice { get; set; }
        public decimal FullLifePrice { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
