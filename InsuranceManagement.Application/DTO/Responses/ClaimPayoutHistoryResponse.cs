namespace InsuranceManagement.Application.DTO.Responses
{
    public class ClaimPayoutHistoryResponse
    {
        public Guid ClaimId { get; set; }
        public string ClientFullName { get; set; } = null!;
        //public string ClientFirstName { get; set; } = null!;
        //public string ClientFatherName { get; set; } = null!;
        //public string ClientGrandFatherName { get; set; } = null!;
        public string ClientPhoneNumber { get; set; } = null!;
        public string ReceiverName { get; set; }
        public string ReceiverPhoneNumber { get; set; }


        public decimal ApprovedAmount { get; set; }
        public DateTime PaidAt { get; set; }
        public string PayoutReference { get; set; } = null!;
        public string ClaimStatus { get; set; } = null!;
        public string PayoutStatus { get; set; } = null!;
        public string? ClaimDescription { get; set; }
        public DateTime IncidentDate { get; set; }
        public string? ClaimType { get; set; }
    }
}
