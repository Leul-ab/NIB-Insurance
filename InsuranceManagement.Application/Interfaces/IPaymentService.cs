using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentInitializeResponse> InitializePaymentAsync(Guid applicationId);
        Task<string> VerifyPaymentAsync(string txRef);
        Task<List<PaymentTransactionResponse>> GetRecentTransactionsAsync();
        Task<List<PaymentTransactionResponse>> RecentTransactionsByUserAsync(Guid clientId);
        Task<List<MotorInsuranceApplicationResponse>> PaidApplicationsByUserAsync(Guid clientId);
        Task<List<LifeInsuranceApplicationResponse>> PaidLifeApplicationsByUserAsync(Guid userId);
        Task<RevealSecretKeyResponse> RevealSecretKeyAsync(Guid userId, RevealSecretKeyRequest request);
        
        Task<List<ClaimPayoutResponse>> PayoutApprovedClaimAsync(Guid claimId);
        Task<List<ClaimPayoutHistoryResponse>> GetPayoutHistoryAsync();

        Task<List<ClaimPayoutHistoryResponse>> GetLifeClaimPayoutHistoryAsync(
    string? secretKey = null,
    string? beneficiaryPhone = null);
        //Task<PaymentResult> SendToPhoneAsync(decimal amount,string phoneNumber,string reference,string description);
        //Task<bool> HandleWebhookAsync(ChapaWebhookRequest request);
    }
}
