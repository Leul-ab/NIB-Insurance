using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace InsuranceManagement.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_settings.SmtpServer, _settings.Port))
            {
                client.Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_settings.FromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // 👈 THIS ENABLES HTML RENDERING
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }

        public async Task SendOtpEmailAsync(string to, string otpCode)
        {
            var subject = "Password Reset OTP Code";
            var body = $"Your OTP code is <b>{otpCode}</b>. It will expire in 10 minutes.";
            await SendEmailAsync(to, subject, body);
        }

    }
}
