using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace InsuranceManagement.Application.DTO.Responses
{
    public class ClaimResponse
    {
        public Guid ClaimId { get; set; }
        public Guid ClientId { get; set; }
        public Guid? MotorInsuranceApplicationId { get; set; }
        public Guid? LifeInsuranceApplicationId { get; set; }

        public DateTime IncidentDate { get; set; }
        public TimeSpan? IncidentTime { get; set; }
        public string? Location { get; set; }
        public IncidentType? IncidentType { get; set; }
        public string? Description { get; set; }

        public List<string>? EvidenceImageUrls { get; set; }
        public string? TrafficPoliceReportPdfUrl { get; set; }

        public string? ClaimReason { get; set; }

        public string? DeathCertificatePdf { get; set; }
        public string? MedicalReportPdf { get; set; }
        public string? HospitalDischargeSummaryPdf { get; set; }

        public string? HospitalName { get; set; }

        public ClaimStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? ClientFirstName { get; set; }
        public string? ClientFatherName { get; set; }
        public string? ClientGrandFatherName { get; set; }
        public string? ClientEmail { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string? RejectionReason { get; set; }



        //Beneficiary
        //public string? BeneficiaryName { get; set; }
        //public BeneficiaryRelation BeneficiaryRelation { get; set; }
        //public string? BeneficiaryPhoneNumber { get; set; }
        //public string? BeneficiaryNationalIdPath { get; set; }
        //public string? BeneficiryEmail { get; set; }
        public List<BeneficiaryResponse> Beneficiaries { get; set; }
    }

}
