using System.Threading.Tasks;

namespace InsuranceManagement.Application.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Sends a generic email.
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="body">Email body (HTML or plain text)</param>
        Task SendEmailAsync(string to, string subject, string body);

        /// <summary>
        /// Sends a one-time password (OTP) email for password reset.
        /// </summary>
        /// <param name="to">Recipient email address</param>
        /// <param name="otpCode">OTP code to include in the email</param>
        Task SendOtpEmailAsync(string to, string otpCode);
    }
}
