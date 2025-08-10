

using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTOs.Responses;

namespace InsuranceManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        //Task<bool> SendPasswordResetOtpAsync(SendOtpRequest request);

        //Task<bool> VerifyOtpAsync(VerifyOtpRequest request);
        //Task<bool> ResetPasswordAsync(string email, string newPassword);
        //Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request);

        
    }

}
