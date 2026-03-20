using InsuranceManagement.Domain.Enums;

namespace InsuranceManagement.Domain.Entities
{
    public class ClaimR
    {
        public Guid Id { get; set; }

        // ========================
        // Relations
        // ========================
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public Finance Finance { get; set; } = null!;

        // Operator who reviewed the claim (OPTIONAL)
        public Guid? OperatorId { get; set; }
        public Guid? FinanceId { get; set; }
        public Operator? Operator { get; set; }

        public Guid? MotorInsuranceApplicationId { get; set; }
        public MotorInsuranceApplication? MotorInsuranceApplication { get; set; }

        public Guid? LifeInsuranceApplicationId { get; set; }
        public LifeInsuranceApplication? LifeInsuranceApplication { get; set; }

        // ========================
        // Incident info
        // ========================
        public DateTime IncidentDate { get; set; }
        public TimeSpan? IncidentTime { get; set; }
        public string? Location { get; set; }
        public IncidentType? IncidentType { get; set; }
        public string Description { get; set; } = null!;

        // ========================
        // Attachments
        // ========================
        public string? EvidenceImagesJson { get; set; }
        public string? TrafficPoliceReportPdfUrl { get; set; }

        // ========================
        // Life insurance
        // ========================
        public string? ClaimReason { get; set; }
        public string? DeathCertificatePdf { get; set; }
        public string? MedicalReportPdf { get; set; }
        public string? HospitalDischargeSummaryPdf { get; set; }
        public string? HospitalName { get; set; }

        // ========================
        // Review & Approval
        // ========================
        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public decimal? ApprovedAmount { get; set; }

        // Metadata only (NO FK)
        public DateTime? ApprovedAt { get; set; }
        public string? RejectReason { get; set; }
        public string? RejectionReason { get; set; }

        // ========================
        // Manager approval
        // ========================
        public Guid? ApprovedByManagerId { get; set; }
        public DateTime? ApprovedByManagerAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public Guid? RejectedByManagerId { get; set; }
        public DateTime? RejectedByManagerAt { get; set; }



        //Chapa payout
        public ClaimPayoutStatus PayoutStatus { get; set; } = ClaimPayoutStatus.NotInitiated;
        public DateTime? PaidAt { get; set; }
        public string? PayoutReference { get; set; }
        public string? PayoutFailureReason { get; set; }



        public string? ClientFirstName { get; set; }
        public string? ClientFatherName { get; set; }
        public string? ClientGrandFatherName { get; set; }
        public string? ClientEmail { get; set; }



    }
}
