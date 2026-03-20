using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace InsuranceManagement.Application.DTO.Requests
{
    public class ClaimReportRequest
    {


        public DateTime IncidentDate { get; set; }
        public TimeSpan IncidentTime { get; set; }
        public string Location { get; set; }
        public IncidentType IncidentType { get; set; }
        public string Description { get; set; }

        public List<IFormFile>? EvidenceImages { get; set; }
        public IFormFile? TrafficPoliceReportPdf { get; set; }
    }
}
