namespace InsuranceManagement.Application.DTO.Responses
{
    public class OperatorDashboardResponse
    {
        public int TotalClaims { get; set; }
        public int ApprovedByOperator { get; set; }
        public int RejectedClaims { get; set; }
        public int PendingClaims { get; set; }
    }
}
