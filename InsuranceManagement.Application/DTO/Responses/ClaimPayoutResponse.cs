namespace InsuranceManagement.Application.DTO.Responses
{
    public class ClaimPayoutResponse
    {
        public Guid ClaimId { get; set; }
        public string ClientFullName { get; set; } = null!;
        public string ClientPhoneNumber { get; set; } = null!;
        public string ReceiverName { get; set; }   // Client or Beneficiary
        public string ReceiverPhoneNumber { get; set; }
        public decimal ApprovedAmount { get; set; }
        public DateTime PaidAt { get; set; }
        public string PayoutReference { get; set; } = null!;
        public string ClaimStatus { get; set; } = null!;
        public string PayoutStatus { get; set; } = null!;
    }
}
