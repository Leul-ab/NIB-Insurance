using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class MotorClaimReportRequest : BaseClaimReportRequest
    {
        public TimeSpan? IncidentTime { get; set; }
        public string? Location { get; set; }
        public IncidentType? IncidentType { get; set; }

        public List<IFormFile>? EvidenceImages { get; set; }
        public IFormFile? TrafficPoliceReportPdf { get; set; }
    }

}
