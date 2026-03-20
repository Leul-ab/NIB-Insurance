using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class MotorInsuranceApplyRequest
    {
        public Guid CategoryId { get; set; }
        public Guid SubCategoryId { get; set; }

        public string Model { get; set; } = default!;
        public string PlateNumber { get; set; } = default!;
        public int YearOfManufacture { get; set; }
        public string EngineNumber { get; set; } = default!;
        public string ChassisNumber { get; set; } = default!;
        public InsuranceType InsuranceType { get; set; }

        public IFormFile CarImage { get; set; }
        public IFormFile CarLibreImage { get; set; }
        //public decimal MarketPrice { get; set; }
    }



}
