namespace InsuranceManagement.Application.DTO.Requests
{
    public class ChapaWebhookRequest
    {
        public string TxRef { get; set; }            // Transaction reference you sent
        public string Status { get; set; }           // "success", "failed"
        public string Amount { get; set; }           // Amount paid
        public string Currency { get; set; }         // ETB
        public string Charge { get; set; }           // Chapa charge
        public string CreatedAt { get; set; }        // Webhook timestamp
        public string UpdatedAt { get; set; }
        public string Reference { get; set; }        // same as TxRef (depends on Chapa format)
        public string Message { get; set; }          // e.g. "Payment successful"
    }
}
