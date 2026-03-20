using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class BeneficiaryRequest
    {
        public string Name { get; set; }
        public BeneficiaryRelation Relation { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        // Files will need IFormFile if needed
        public IFormFile? NationalIdFile { get; set; }
    }
}
