using InsuranceManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Domain.Entities
{
    public class MotorInsuranceApplication
    {
        public Guid Id { get; set; }

        // link to client
        public Guid ClientId { get; set; }
        public Client Client { get; set; }

        // selected category + subcategory
        public Guid CategoryId { get; set; }
        public OperatorCategory Category { get; set; }

        public Guid SubCategoryId { get; set; }
        public OperatorCategory SubCategory { get; set; }

        // Motor-specific data
        public string Model { get; set; } = default!;
        public string PlateNumber { get; set; } = default!;
        public int YearOfManufacture { get; set; }
        public string EngineNumber { get; set; } = default!;
        public string ChassisNumber { get; set; } = default!;
        public decimal MarketPrice { get; set; }
        public string? Message { get; set; }
        // Calculated Insurance price (2.5%)
        public decimal CalculatedPremium { get; set; }
        public InsuranceType InsuranceType { get; set; } 

        // system
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Pending"; // approved/rejected later
        public string PaymentStatus { get; set; } = "Unpaid"; // paid/unpaid later

        //payment
        public string? PaymentReference { get; set; }
        public bool IsPaid { get; set; } = false;

        public string CarImagePath { get; set; }
        public string CarLibreImagePath { get; set; }
    }

}
