using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Domain.Enums;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InsuranceManagement.Infrastructure.Services
{
    public class ManagerService : IManagerService
    {
        private readonly InsuranceDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IFileStorageService _fileStorage;

        public ManagerService(InsuranceDbContext context, IEmailService emailService, IFileStorageService fileStorage)
        {
            _context = context;
            _emailService = emailService;
            _fileStorage = fileStorage;
        }

        public async Task<ManagerResponse> AddManagerAsync(ManagerRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email already exists.");

            var passwordHasher = new PasswordHasher<User>();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                Role = UserRole.Manager,
                PasswordHash = passwordHasher.HashPassword(null, request.Password) // ✅ hash the actual password
            };

            // Upload profile image to Cloudinary — returns permanent HTTPS URL
            var logoImageUrl = await _fileStorage.UploadAsync(request.LogoImageUrl, "managers");


            var manager = new Manager
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFatherName = request.GrandFatherName,
                LogoImageUrl = logoImageUrl,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                PhoneNumber = request.PhoneNumber,
                Region = request.Region,
                City = request.City,
                SubCity = request.SubCity,
                NationalIdOrPassport = request.NationalIdOrPassport,
                CreatedAt = request.CreatedAt == default ? DateTime.UtcNow : request.CreatedAt,
                User = user
            };

            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            return new ManagerResponse
            {
                Id = manager.Id,
                FirstName = manager.FirstName,
                FatherName = manager.FatherName,
                GrandFatherName = manager.GrandFatherName,
                LogoImageUrl = manager.LogoImageUrl,
                Email = manager.User.Email,
                DateOfBirth = manager.DateOfBirth,
                Gender = (Gender)manager.Gender,
                PhoneNumber = manager.PhoneNumber,
                Region = manager.Region,
                City = manager.City,
                SubCity = manager.SubCity,
                NationalIdOrPassport = manager.NationalIdOrPassport,
                CreatedAt = manager.CreatedAt
            };
        }

        public async Task<IEnumerable<ManagerResponse>> GetAllManagersAsync()
        {
            var managers = await _context.Managers.Include(m => m.User).ToListAsync();

            return managers.Select(m => new ManagerResponse
            {
                Id = m.Id,
                FirstName = m.FirstName,
                FatherName = m.FatherName,
                GrandFatherName = m.GrandFatherName,
                Email = m.User.Email,
                DateOfBirth = m.DateOfBirth,
                Gender = (Gender)m.Gender,
                PhoneNumber = m.PhoneNumber,
                Region = m.Region,
                City = m.City,
                SubCity = m.SubCity,
                NationalIdOrPassport = m.NationalIdOrPassport,
                CreatedAt = m.CreatedAt
            });
        }

        public async Task<ManagerResponse?> GetManagerByIdAsync(Guid id)
        {
            var manager = await _context.Managers.Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null) return null;

            return new ManagerResponse
            {
                Id = manager.Id,
                FirstName = manager.FirstName,
                FatherName = manager.FatherName,
                GrandFatherName = manager.GrandFatherName,
                Email = manager.User.Email,
                DateOfBirth = manager.DateOfBirth,
                Gender = (Gender)manager.Gender,
                PhoneNumber = manager.PhoneNumber,
                Region = manager.Region,
                City = manager.City,
                SubCity = manager.SubCity,
                NationalIdOrPassport = manager.NationalIdOrPassport,
                CreatedAt = manager.CreatedAt
            };
        }

        public async Task<ManagerResponse> UpdateManagerAsync(Guid managerId, ManagerRequest request)
        {
            var manager = await _context.Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == managerId);

            if (manager == null)
                throw new Exception("Manager not found.");

            // Prevent duplicate email
            if (manager.User.Email != request.Email &&
                await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email already exists.");

            // Update User
            manager.User.Email = request.Email;
            manager.User.UserName = request.Email;

            // Update Manager
            manager.FirstName = request.FirstName;
            manager.FatherName = request.FatherName;
            manager.GrandFatherName = request.GrandFatherName;
            manager.DateOfBirth = request.DateOfBirth;
            manager.Gender = request.Gender;
            manager.PhoneNumber = request.PhoneNumber;
            manager.Region = request.Region;
            manager.City = request.City;
            manager.SubCity = request.SubCity;
            manager.NationalIdOrPassport = request.NationalIdOrPassport;

            // Optional password update
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                manager.User.PasswordHash =
                    passwordHasher.HashPassword(manager.User, request.Password);
            }

            await _context.SaveChangesAsync();

            // ✅ Send email after update
            var emailSubject = "Profile Updated Successfully";
            var emailBody = $@"
        Hello {manager.FirstName},<br/><br/>
        Your manager profile has been updated successfully.<br/>
        If you did not make this change, please contact support immediately.<br/><br/>
        Regards,<br/>
        NIB Team";

            await _emailService.SendEmailAsync(manager.User.Email, emailSubject, emailBody);

            return new ManagerResponse
            {
                Id = manager.Id,
                FirstName = manager.FirstName,
                FatherName = manager.FatherName,
                GrandFatherName = manager.GrandFatherName,
                Email = manager.User.Email,
                DateOfBirth = manager.DateOfBirth,
                Gender = (Gender)manager.Gender,
                PhoneNumber = manager.PhoneNumber,
                Region = manager.Region,
                City = manager.City,
                SubCity = manager.SubCity,
                NationalIdOrPassport = manager.NationalIdOrPassport,
                CreatedAt = manager.CreatedAt
            };
        }


        public async Task<bool> DeleteManagerAsync(Guid managerId)
        {
            var manager = await _context.Managers
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == managerId);

            if (manager == null)
                throw new Exception("Manager not found.");

            _context.Users.Remove(manager.User);
            _context.Managers.Remove(manager);

            await _context.SaveChangesAsync();
            return true;
        }






        public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                throw new Exception("New password is required.");

            if (request.NewPassword != request.ConfirmPassword)
                throw new Exception("Passwords do not match.");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found.");

            var hasher = new PasswordHasher<User>();

            var verifyResult = hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.CurrentPassword
            );

            if (verifyResult == PasswordVerificationResult.Failed)
                throw new Exception("Current password is incorrect.");

            user.PasswordHash = hasher.HashPassword(user, request.NewPassword);

            await _context.SaveChangesAsync();
        }

        public async Task<ManagerResponse?> GetMyProfileAsync(Guid userId)
        {
            var client = await _context.Managers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null) return null;

            return new ManagerResponse
            {
                Id = client.Id,
                FirstName = client.FirstName,
                FatherName = client.FatherName,
                GrandFatherName = client.GrandFatherName,
                DateOfBirth = client.DateOfBirth,
                Gender = (Gender)client.Gender,
                NationalIdOrPassport = client.NationalIdOrPassport,
                LogoImageUrl = client.LogoImageUrl,
                Email = client.User?.Email ?? string.Empty,
                PhoneNumber = client.PhoneNumber,
                Region = client.Region,
                City = client.City,
                SubCity = client.SubCity,
                CreatedAt = client.CreatedAt
            };
        }

        public async Task<ApproveClaimResponse> ApproveReviewedClaimAsync(Guid managerUserId, Guid claimId)

        {
            var claim = await _context.Claims
                .Include(c => c.Client).ThenInclude(cl => cl.User)
                .Include(c => c.MotorInsuranceApplication)
                .Include(c => c.LifeInsuranceApplication).ThenInclude(l => l.Category)
                .FirstOrDefaultAsync(c => c.Id == claimId);

            if (claim == null)
                throw new Exception("Claim not found.");

            if (claim.Status != ClaimStatus.ReviewedByOperator)
                throw new Exception("Claim is not ready for manager approval.");

            

            // ✅ Manager approves
            claim.Status = ClaimStatus.ApprovedByManager;
            claim.ApprovedByManagerId = managerUserId;
            claim.ApprovedByManagerAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // =========================
            // 📧 EMAILS (keep EXACTLY as you sent)
            // =========================

            if (claim.MotorInsuranceApplicationId != null)
            {
                var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Motor Claim Approved</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #2E86C1; text-align: center;'>Motor Claim Approved</h2>
        <p>Dear <strong>{claim.Client.FirstName}</strong>,</p>
        <p>We are pleased to inform you that your motor claim (ID: <strong>{claim.Id}</strong>) has been <span style='color: green; font-weight: bold;'>approved</span> by our manager.</p>

        <h3 style='color: #2E86C1;'>Claim Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Claim ID</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.Id}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Approved Amount</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.ApprovedAmount:C}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Approved At</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.ApprovedByManagerAt.Value:f}</td>
            </tr>
        </table>

        <p style='margin-top: 20px;'>You can now proceed with the settlement process.</p>
        <p style='margin-top: 30px;'>Thank you for trusting <strong>Your Insurance Company Name</strong>!</p>
    </div>
</body>
</html>
";

                await _emailService.SendEmailAsync(claim.Client.User.Email,
                    "Motor Claim Approved", emailBody);
            }
            else if (claim.LifeInsuranceApplicationId != null)
            {
                var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Life Claim Approved</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #2E86C1; text-align: center;'>Life Claim Approved</h2>
        <p>Dear <strong>{claim.Client.FirstName}</strong>,</p>
        <p>We are pleased to inform you that your life insurance claim (ID: <strong>{claim.Id}</strong>) has been <span style='color: green; font-weight: bold;'>approved</span> by our manager.</p>

        <h3 style='color: #2E86C1;'>Claim Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Claim ID</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.Id}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Approved Amount</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.ApprovedAmount:C}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Approved At</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{DateTime.UtcNow:f}</td>
            </tr>
        </table>

        <p style='margin-top: 20px;'>You can now proceed with the settlement process.</p>
        <p style='margin-top: 30px;'>Thank you for trusting <strong>Your Insurance Company Name</strong>!</p>
    </div>
</body>
</html>
";

                await _emailService.SendEmailAsync(claim.Client.User.Email,
                    "Life Claim Approved", emailBody);
            }

            return new ApproveClaimResponse
            {
                ClaimId = claim.Id,
                Status = claim.Status,
                ApprovedAmount = claim.ApprovedAmount ?? 0m,
                ApprovedAt = claim.ApprovedByManagerAt!.Value
            };
        }

        public async Task<RejectClaimResponse> RejectReviewedClaimAsync(Guid managerUserId, Guid claimId, ManagerRejectClaimRequest request)
        {
            var claim = await _context.Claims
                .Include(c => c.Client).ThenInclude(cl => cl.User)
                .Include(c => c.LifeInsuranceApplication)
                .FirstOrDefaultAsync(c => c.Id == claimId);

            if (claim == null)
                throw new Exception("Claim not found.");

            if (claim.Status != ClaimStatus.ReviewedByOperator)
                throw new Exception("Only claims reviewed by operator can be rejected by manager.");

            if (string.IsNullOrWhiteSpace(request.RejectionReason))
                throw new Exception("Rejection reason is required.");

            claim.Status = ClaimStatus.RejectedByManager;
            claim.RejectionReason = request.RejectionReason;
            claim.RejectedByManagerId = managerUserId;
            claim.RejectedByManagerAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // ================= EMAIL SECTION (UNCHANGED CONTENT) =================

            if (claim.LifeInsuranceApplicationId == null)
            {
                // MOTOR CLAIM EMAIL
                var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Motor Claim Rejected</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #E74C3C; text-align: center;'>Motor Claim Rejected</h2>
        <p>Dear <strong>{claim.Client.FirstName}</strong>,</p>
        <p>We regret to inform you that your motor claim (ID: <strong>{claim.Id}</strong>) has been <span style='color: red; font-weight: bold;'>rejected</span> by our manager.</p>

        <h3 style='color: #2E86C1;'>Claim Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Claim ID</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.Id}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Rejection Reason</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.RejectionReason}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Rejected At</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.RejectedByManagerAt.Value:f}</td>
            </tr>
        </table>

        <p style='margin-top: 20px;'>If you have questions, please contact our support team.</p>
        <p style='margin-top: 30px;'>Thank you for trusting <strong>Your Insurance Company Name</strong>.</p>
    </div>
</body>
</html>
";

                await _emailService.SendEmailAsync(
                    claim.Client.User.Email,
                    "Motor Claim Rejected",
                    emailBody);
            }
            else
            {
                // LIFE CLAIM EMAIL
                var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Life Claim Rejected</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #E74C3C; text-align: center;'>Life Claim Rejected</h2>
        <p>Dear <strong>{claim.Client.FirstName}</strong>,</p>
        <p>We regret to inform you that your life insurance claim (ID: <strong>{claim.Id}</strong>) has been <span style='color: red; font-weight: bold;'>rejected</span> by our manager.</p>

        <h3 style='color: #2E86C1;'>Claim Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Claim ID</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.Id}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Rejection Reason</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{claim.RejectionReason}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Rejected At</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{DateTime.UtcNow:f}</td>
            </tr>
        </table>

        <p style='margin-top: 20px;'>If you have questions, please contact our support team.</p>
        <p style='margin-top: 30px;'>Thank you for trusting <strong>Your Insurance Company Name</strong>.</p>
    </div>
</body>
</html>
";

                await _emailService.SendEmailAsync(
                    claim.Client.User.Email,
                    "Life Claim Rejected",
                    emailBody);
            }

            return new RejectClaimResponse
            {
                ClaimId = claim.Id,
                Status = claim.Status,
                RejectionReason = claim.RejectionReason,
                RejectedAt = claim.RejectedByManagerAt.Value
            };
        }

        public async Task<List<ReviewedClaimDto>> GetClaimsReviewedByOperatorsAsync()
        {
            // Get all claims that are in ReviewedByOperator status
            var claims = await _context.Claims
                .Include(c => c.Client)
                .Include(o => o.Operator)
                .Where(c => c.Status == ClaimStatus.ReviewedByOperator)
                .OrderBy(c => c.ApprovedAt)
                .ToListAsync();

            // Map to DTO
            var result = claims.Select(c => new ReviewedClaimDto
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                ClientName = $"{c.Client.FirstName} {c.Client.FatherName}",
                OperatorName = $"{c.Operator.FirstName} {c.Operator.FatherName}",
                IncidentDate = c.IncidentDate,
                Location = c.Location ?? string.Empty,
                IncidentType = c.IncidentType?.ToString() ?? string.Empty,
                ProposedAmount = c.ApprovedAmount ?? 0,
              
                ReviewedAt = c.ApprovedAt ?? DateTime.MinValue,
                //ReviewedByOperatorId = c.ApprovedByOperatorId ?? Guid.Empty
            }).ToList();

            return result;
        }

        public async Task<List<ClaimResponse>> GetAllReviewedByOperatorLifeClaimsAsync()
        {
            var claims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.ReviewedByOperator &&
                            c.LifeInsuranceApplicationId != null)
                .Include(c => c.LifeInsuranceApplication)
                .ThenInclude(a => a.Beneficiaries)
                .ToListAsync();

            return claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId.Value,
                IncidentDate = c.IncidentDate,
                ClaimReason = c.ClaimReason,
                DeathCertificatePdf = c.DeathCertificatePdf,
                MedicalReportPdf = c.MedicalReportPdf,
                HospitalDischargeSummaryPdf = c.HospitalDischargeSummaryPdf,
                HospitalName = c.HospitalName,
                Description = c.Description,
                Status = c.Status,
                CreatedAt = c.CreatedAt,

                // Client Info
                ClientEmail = c.ClientEmail,
                ClientFirstName = c.ClientFirstName,
                ClientFatherName = c.ClientFatherName,
                ClientGrandFatherName = c.ClientGrandFatherName,
                ApprovedAmount = c.ApprovedAmount,

                // Multiple beneficiaries returned as list
                Beneficiaries = c.LifeInsuranceApplication.Beneficiaries
                    .Select(b => new BeneficiaryResponse
                    {
                        Name = b.Name,
                        Email = b.Email,
                        Relation = b.Relation,
                        PhoneNumber = b.PhoneNumber,
                        NationalIdFilePath = b.NationalIdFilePath
                    }).ToList()

            }).ToList();
        }

        public async Task<List<ReviewedClaimDto>> GetApprovedReviewedClaimsAsync()
        {
            var claims = await _context.Claims
                .Include(c => c.Client)
                .Include(c => c.Operator)
                .Where(c => c.Status == ClaimStatus.ApprovedByManager)
                .OrderByDescending(c => c.ApprovedByManagerAt)
                .ToListAsync();

            return claims.Select(c => new ReviewedClaimDto
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                ClientName = $"{c.Client.FirstName} {c.Client.FatherName}",
                OperatorName = $"{c.Operator.FirstName} {c.Operator.FatherName}",
                IncidentDate = c.IncidentDate,
                Location = c.Location ?? string.Empty,
                IncidentType = c.IncidentType?.ToString() ?? string.Empty,
                ProposedAmount = c.ApprovedAmount ?? 0,
                ReviewedAt = c.ApprovedAt ?? DateTime.MinValue,
                //ReviewedByOperatorId = c.ApprovedByOperatorId ?? Guid.Empty
            }).ToList();
        }

        public async Task<List<ReviewedClaimDto>> GetRejectedReviwedClaimsAsync()
        {
            var claims = await _context.Claims
                .Include(c => c.Client)
                .Include(c => c.Operator)
                .Where(c => c.Status == ClaimStatus.RejectedByManager)
                .OrderByDescending(c => c.RejectedByManagerAt)
                .ToListAsync();

            return claims.Select(c => new ReviewedClaimDto
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                ClientName = $"{c.Client.FirstName} {c.Client.FatherName}",
                OperatorName = $"{c.Operator.FirstName} {c.Operator.FatherName}",
                IncidentDate = c.IncidentDate,
                Location = c.Location ?? string.Empty,
                IncidentType = c.IncidentType?.ToString() ?? string.Empty,
                ProposedAmount = c.ApprovedAmount ?? 0,
                ReviewedAt = c.ApprovedAt ?? DateTime.MinValue,
                //ReviewedByOperatorId = c.ApprovedByOperatorId ?? Guid.Empty
            }).ToList();
        }
        
        public async Task<List<ClaimResponse>> GetAllLifeApprovedByManagerAsync()
        {
            var claims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.ApprovedByManager &&
                            c.LifeInsuranceApplicationId != null)
                .Include(c => c.LifeInsuranceApplication)
                    .ThenInclude(a => a.Beneficiaries)
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.User)
                .ToListAsync();


            return claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId,
                IncidentDate = c.IncidentDate,
                ClaimReason = c.ClaimReason,
                DeathCertificatePdf = c.DeathCertificatePdf,
                MedicalReportPdf = c.MedicalReportPdf,
                HospitalDischargeSummaryPdf = c.HospitalDischargeSummaryPdf,
                HospitalName = c.HospitalName,
                Description = c.Description,
                Status = c.Status,
                ApprovedAmount = c.ApprovedAmount,
                RejectionReason = c.RejectionReason,
                CreatedAt = c.CreatedAt,

                // Client info
                ClientFirstName = c.Client.FirstName,
                ClientFatherName = c.Client.FatherName,
                ClientGrandFatherName = c.Client.GrandFatherName,
                ClientEmail = c.Client.User.Email,

                // Beneficiaries
                Beneficiaries = c.LifeInsuranceApplication!.Beneficiaries.Select(b => new BeneficiaryResponse
                {
                    Name = b.Name,
                    Relation = b.Relation,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    NationalIdFilePath = b.NationalIdFilePath
                }).ToList()
            }).ToList();

        }

        public async Task<List<ClaimResponse>> GetAllLifeRejectedByManagerAsync()
        {
            var claims = await _context.Claims
                .Where(c => c.Status == ClaimStatus.Rejected &&
                            c.LifeInsuranceApplicationId != null)
                .Include(c => c.Client)
                    .ThenInclude(cl => cl.User) // Load client email
                .Include(c => c.LifeInsuranceApplication)
                    .ThenInclude(a => a.Beneficiaries) // Load beneficiaries
                .ToListAsync();

            return claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId!.Value,
                IncidentDate = c.IncidentDate,
                ClaimReason = c.ClaimReason,
                DeathCertificatePdf = c.DeathCertificatePdf,
                MedicalReportPdf = c.MedicalReportPdf,
                HospitalDischargeSummaryPdf = c.HospitalDischargeSummaryPdf,
                HospitalName = c.HospitalName,
                Description = c.Description,
                Status = c.Status,
                ApprovedAmount = c.ApprovedAmount,
                RejectionReason = c.RejectionReason,
                CreatedAt = c.CreatedAt,

                // Client info
                ClientFirstName = c.Client.FirstName,
                ClientFatherName = c.Client.FatherName,
                ClientGrandFatherName = c.Client.GrandFatherName,
                ClientEmail = c.Client.User.Email,

                // Beneficiaries
                Beneficiaries = c.LifeInsuranceApplication!.Beneficiaries.Select(b => new BeneficiaryResponse
                {
                    Name = b.Name,
                    Relation = b.Relation,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    NationalIdFilePath = b.NationalIdFilePath
                }).ToList()
            }).ToList();
        }

        public async Task<ReviewedClaimDto> GetReviewedClaimByIdAsync(Guid claimId)
        {
            var claim = await _context.Claims
                .Include(c => c.Client)
                .Include(c => c.Operator)
                .FirstOrDefaultAsync(c => c.Id == claimId &&
                                         (c.Status == ClaimStatus.ReviewedByOperator ||
                                          c.Status == ClaimStatus.ApprovedByManager));

            if (claim == null)
                throw new Exception("Reviewed claim not found.");

            return new ReviewedClaimDto
            {
                ClaimId = claim.Id,
                ClientId = claim.ClientId,
                ClientName = $"{claim.Client.FirstName} {claim.Client.FatherName}",
                OperatorName = $"{claim.Operator.FirstName} {claim.Operator.FatherName}",
                IncidentDate = claim.IncidentDate,
                Location = claim.Location ?? string.Empty,
                IncidentType = claim.IncidentType?.ToString() ?? string.Empty,
                ProposedAmount = claim.ApprovedAmount ?? 0,
                ReviewedAt = claim.ApprovedAt ?? DateTime.MinValue,
                //ReviewedByOperatorId = claim.ApprovedByOperatorId ?? Guid.Empty
            };
        }

        public async Task<ClaimResponse> GetReviewedLifeClaimByIdAsync(Guid claimId)
        {
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c =>
                    c.Id == claimId &&
                    c.Status == ClaimStatus.ReviewedByOperator &&
                    c.LifeInsuranceApplicationId != null)
                ?? throw new Exception("Reviewed claim not found for manager.");

            return new ClaimResponse
            {
                ClaimId = claim.Id,
                ClientId = claim.ClientId,
                LifeInsuranceApplicationId = claim.LifeInsuranceApplicationId,
                IncidentDate = claim.IncidentDate,
                ClaimReason = claim.ClaimReason,
                DeathCertificatePdf = claim.DeathCertificatePdf,
                MedicalReportPdf = claim.MedicalReportPdf,
                HospitalDischargeSummaryPdf = claim.HospitalDischargeSummaryPdf,
                HospitalName = claim.HospitalName,
                Description = claim.Description,
                Status = claim.Status,
                ApprovedAmount = claim.ApprovedAmount,
                RejectionReason = claim.RejectionReason,
                CreatedAt = claim.CreatedAt,
            };
        }

        public async Task<List<ClaimPayoutManagerResponse>> GetAllPayoutsForManagerAsync()
        {
            var payouts = await _context.Claims
                .Include(c => c.Client)
                .Include(c => c.Finance) // finance/operator who processed payout
                .Where(c => c.PayoutStatus == ClaimPayoutStatus.Paid || c.PayoutStatus == ClaimPayoutStatus.Processing)
                .OrderByDescending(c => c.PaidAt)
                .ToListAsync();

            return payouts.Select(c => new ClaimPayoutManagerResponse
            {
                ClaimId = c.Id,
                ClientFullName = $"{c.Client.FirstName} {c.Client.FatherName} {c.Client.GrandFatherName}",
                ClientPhoneNumber = c.Client.PhoneNumber,
                ApprovedAmount = c.ApprovedAmount ?? 0,
                PaidAt = c.PaidAt ?? DateTime.MinValue,
                PayoutReference = c.PayoutReference ?? "",
                ClaimStatus = c.Status.ToString(),
                PayoutStatus = c.PayoutStatus.ToString(),
                ClaimDescription = c.Description,
                IncidentDate = c.IncidentDate,
                ClaimType = c.IncidentType?.ToString(),
                ProcessedBy = c.Operator != null
                    ? $"{c.Finance.FirstName} {c.Finance.FatherName}"
                    : "System/Not assigned"
            }).ToList();
        }

        public async Task<ManagerDashboardStatsDto> GetManagerDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var startOfYear = new DateTime(DateTime.UtcNow.Year, 1, 1);

            var totalClaims = await _context.Claims.CountAsync();

            var reviewedByOperatorCount = await _context.Claims
                .CountAsync(c => c.Status == ClaimStatus.ReviewedByOperator);

            var approvedByManagerCount = await _context.Claims
                .CountAsync(c => c.Status == ClaimStatus.ApprovedByManager);

            var rejectedByManagerCount = await _context.Claims
                .CountAsync(c => c.Status == ClaimStatus.RejectedByManager);

            var lifeClaimsCount = await _context.Claims
                .CountAsync(c => c.LifeInsuranceApplicationId != null);

            var motorClaimsCount = await _context.Claims
                .CountAsync(c => c.MotorInsuranceApplicationId != null);

            var totalApprovedAmount = await _context.Claims
                .Where(c => c.Status == ClaimStatus.ApprovedByManager && c.ApprovedAmount != null)
                .SumAsync(c => c.ApprovedAmount!.Value);

            var approvedThisMonthAmount = await _context.Claims
                .Where(c => c.Status == ClaimStatus.ApprovedByManager &&
                            c.ApprovedByManagerAt >= startOfMonth &&
                            c.ApprovedAmount != null)
                .SumAsync(c => c.ApprovedAmount!.Value);

            var approvedThisYearAmount = await _context.Claims
                .Where(c => c.Status == ClaimStatus.ApprovedByManager &&
                            c.ApprovedByManagerAt >= startOfYear &&
                            c.ApprovedAmount != null)
                .SumAsync(c => c.ApprovedAmount!.Value);



            var reviewedTodayCount = await _context.Claims
                .CountAsync(c => c.Status == ClaimStatus.ReviewedByOperator &&
                                 c.ApprovedAt != null &&
                                 c.ApprovedAt.Value.Date == today);

            return new ManagerDashboardStatsDto
            {
                TotalClaims = totalClaims,
                ReviewedByOperatorCount = reviewedByOperatorCount,
                ApprovedByManagerCount = approvedByManagerCount,
                RejectedByManagerCount = rejectedByManagerCount,
                LifeClaimsCount = lifeClaimsCount,
                MotorClaimsCount = motorClaimsCount,
                TotalApprovedAmount = totalApprovedAmount,
                ApprovedThisMonthAmount = approvedThisMonthAmount,
                ApprovedThisYearAmount = approvedThisYearAmount,
                ReviewedTodayCount = reviewedTodayCount
            };
        }



    }
}
