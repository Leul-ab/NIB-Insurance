using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Application.Interfaces
{
    public interface IClientService
    {
        Task<ClientResponse> RegisterClientAsync(ClientRegisterRequest request);
        Task<ClientResponse> GetClientByIdAsync(Guid id);
        Task<ClientResponse> GetClientByUserIdAsync(Guid userId);
        Task<ClientResponse?> GetMyProfileAsync(Guid userId);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<List<ClientResponse>> GetAllClientsAsync();
        Task<bool> DeleteClientAsync(Guid id);
        Task<ClientResponse> UpdateClientAsync(Guid clientId, ClientUpdateRequest request);
        //Task<ClientResponse?> UpdateClientByEmailAsync(string email, ClientUpdateRequest request);

        // Password / OTP
        Task<bool> SendPasswordResetOtpAsync(ForgotPasswordRequest request);
        Task<bool> VerifyOtpAsync(VerifyOtpRequest request);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
        Task<bool> ResendOtpAsync(ResendOtpRequest request);

        // Motor Insurance
        Task<MotorInsuranceApplicationResponse> PreviewMotorInsuranceAsync(Guid clientId, MotorInsuranceApplyRequest request);
        Task<MotorInsuranceApplicationResponse> ConfirmMotorInsuranceAsync(Guid clientId, MotorInsuranceConfirmRequest request);
        Task<LifeInsuranceApplicationResponse> PreviewLifeInsuranceAsync(Guid clientId, LifeInsuranceApplyRequest request);
        Task<BeneficiaryResponse> AddSingleBeneficiaryLifeInsuranceAsync(
    Guid applicationId,
    BeneficiaryRequest beneficiary);
        Task<LifeInsuranceApplicationResponse> ConfirmLifeInsuranceAsync(Guid clientId, LifeInsuranceConfirmRequest request);

        Task<List<MotorInsuranceApplicationResponse>> GetClientMotorApplicationsAsync(Guid clientId);
        Task<List<LifeInsuranceApplicationResponse>> GetClientLifeApplicationsAsync(Guid clientId);

        Task<ClientDashboardResponse> GetClientDashboardAsync(Guid clientId);

        Task<ClaimResponse> ReportMotorClaimAsync(Guid userId, Guid motorApplicationId, MotorClaimReportRequest request);
        Task<ClaimResponse> ReportLifeClaimAsync(Guid applicationId, LifeClaimReportRequest request);
        Task<VerifyLifePolicyResponse> VerifyLifePolicyAsync(string secretKey, string beneficiaryEmail);

        Task<List<ClaimResponse>> GetMyClaimsAsync(Guid userId);
        Task<ClientClaimsByStatusResponse> GetMyClaimsByStatusAsync(Guid userId);
        Task<List<ClaimPayoutHistoryResponse>> GetClientPayoutHistoryAsync(Guid clientUserId);

    }
}
