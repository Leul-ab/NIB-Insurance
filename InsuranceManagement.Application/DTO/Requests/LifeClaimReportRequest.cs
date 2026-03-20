using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class LifeClaimReportRequest : BaseClaimReportRequest
    {
        public string? ClaimReason { get; set; }  // Death, Critical Illness, Disability

        public IFormFile? DeathCertificatePdf { get; set; }
        public IFormFile? MedicalReportPdf { get; set; }
        public IFormFile? HospitalDischargeSummaryPdf { get; set; }

        public string? HospitalName { get; set; }
    }
}

