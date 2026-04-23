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
    public class FinanceService : IFinanceService
    {
        private readonly InsuranceDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IFileStorageService _fileStorage;

        public FinanceService(InsuranceDbContext context, IEmailService emailService, IFileStorageService fileStorage)
        {
            _context = context;
            _emailService = emailService;
            _fileStorage = fileStorage;
        }

        public async Task<FinanceResponse> AddFinanceAsync(FinanceRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email already exists.");

            var passwordHasher = new PasswordHasher<User>();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                Role = UserRole.Finance,
                PasswordHash = passwordHasher.HashPassword(null, request.Password) // ✅ hash the actual password
            };

            // Upload profile image to Cloudinary — returns permanent HTTPS URL
            var logoImageUrl = await _fileStorage.UploadAsync(request.LogoImageUrl, "finances");

            var finance = new Finance
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

            _context.Finances.Add(finance);
            await _context.SaveChangesAsync();

            return new FinanceResponse
            {
                Id = finance.Id,
                FirstName = finance.FirstName,
                FatherName = finance.FatherName,
                GrandFatherName = finance.GrandFatherName,
                LogoImageUrl = finance.LogoImageUrl,
                Email = finance.User.Email,
                DateOfBirth = finance.DateOfBirth,
                Gender = (Gender)finance.Gender,
                PhoneNumber = finance.PhoneNumber,
                Region = finance.Region,
                City = finance.City,
                SubCity = finance.SubCity,
                NationalIdOrPassport = finance.NationalIdOrPassport,
                CreatedAt = finance.CreatedAt
            };
        }

        public async Task<IEnumerable<FinanceResponse>> GetAllFinancesAsync()
        {
            var finances = await _context.Finances.Include(f => f.User).ToListAsync();

            return finances.Select(f => new FinanceResponse
            {
                Id = f.Id,
                FirstName = f.FirstName,
                FatherName = f.FatherName,
                GrandFatherName = f.GrandFatherName,
                Email = f.User.Email,
                DateOfBirth = f.DateOfBirth,
                Gender = (Gender)f.Gender,
                PhoneNumber = f.PhoneNumber,
                Region = f.Region,
                City = f.City,
                SubCity = f.SubCity,
                NationalIdOrPassport = f.NationalIdOrPassport,
                CreatedAt = f.CreatedAt
            });
        }

        public async Task<FinanceResponse?> GetFinanceByIdAsync(Guid id)
        {
            var finance = await _context.Finances.Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (finance == null) return null;

            return new FinanceResponse
            {
                Id = finance.Id,
                FirstName = finance.FirstName,
                FatherName = finance.FatherName,
                GrandFatherName = finance.GrandFatherName,
                Email = finance.User.Email,
                DateOfBirth = finance.DateOfBirth,
                Gender = (Gender)finance.Gender,
                PhoneNumber = finance.PhoneNumber,
                Region = finance.Region,
                City = finance.City,
                SubCity = finance.SubCity,
                NationalIdOrPassport = finance.NationalIdOrPassport,
                CreatedAt = finance.CreatedAt
            };
        }

        public async Task<FinanceResponse> UpdateFinanceAsync(Guid financeId, FinanceRequest request)
        {
            var finance = await _context.Finances
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == financeId);

            if (finance == null)
                throw new Exception("Finance not found.");

            // Prevent duplicate email
            if (finance.User.Email != request.Email &&
                await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("Email already exists.");

            // Update User
            finance.User.Email = request.Email;
            finance.User.UserName = request.Email;

            // Update Finance info
            finance.FirstName = request.FirstName;
            finance.FatherName = request.FatherName;
            finance.GrandFatherName = request.GrandFatherName;
            finance.DateOfBirth = request.DateOfBirth;
            finance.Gender = request.Gender;
            finance.PhoneNumber = request.PhoneNumber;
            finance.Region = request.Region;
            finance.City = request.City;
            finance.SubCity = request.SubCity;
            finance.NationalIdOrPassport = request.NationalIdOrPassport;

            // Optional password update
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                finance.User.PasswordHash =
                    passwordHasher.HashPassword(finance.User, request.Password);
            }

            await _context.SaveChangesAsync();

            // ✅ Send email after update
            var emailSubject = "Profile Updated Successfully";
            var emailBody = $@"
        Hello {finance.FirstName},<br/><br/>
        Your finance profile has been updated successfully.<br/>
        If you did not make this change, please contact support immediately.<br/><br/>
        Regards,<br/>
        NIB Team";

            await _emailService.SendEmailAsync(finance.User.Email, emailSubject, emailBody);

            return new FinanceResponse
            {
                Id = finance.Id,
                FirstName = finance.FirstName,
                FatherName = finance.FatherName,
                GrandFatherName = finance.GrandFatherName,
                Email = finance.User.Email,
                DateOfBirth = finance.DateOfBirth,
                Gender = (Gender)finance.Gender,
                PhoneNumber = finance.PhoneNumber,
                Region = finance.Region,
                City = finance.City,
                SubCity = finance.SubCity,
                NationalIdOrPassport = finance.NationalIdOrPassport,
                CreatedAt = finance.CreatedAt
            };
        }

        public async Task<bool> DeleteFinanceAsync(Guid financeId)
        {
            var finance = await _context.Finances
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == financeId);

            if (finance == null)
                throw new Exception("Finance not found.");

            _context.Users.Remove(finance.User);
            _context.Finances.Remove(finance);

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

        public async Task<List<MotorInsuranceApplicationResponse>> GetPendingMotorApplicationsAsync()
        {
            var pendingApps = await _context.MotorInsuranceApplications
                .Include(a => a.Client)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Where(a => a.Status == "Pending")
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();

            return pendingApps.Select(app => new MotorInsuranceApplicationResponse
            {
                ApplicationId = app.Id,
                ClientId = app.ClientId,
                CategoryName = app.Category.Name,
                SubCategoryName = app.SubCategory.Name,
                Model = app.Model,
                PlateNumber = app.PlateNumber,
                YearOfManufacture = app.YearOfManufacture,
                MarketPrice = app.MarketPrice,
                CalculatedPremium = app.CalculatedPremium,
                InsuranceType = app.InsuranceType,
                Status = app.Status,
                CreatedAt = app.CreatedAt
            }).ToList();
        }

        public async Task<List<LifeInsuranceApplicationResponse>> GetPendingLifeApplicationsAsync()
        {
            var pendingApps = await _context.LifeInsuranceApplications
                .Include(a => a.Client)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Where(a => a.Status == "Pending")
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();

            return pendingApps.Select(app => new LifeInsuranceApplicationResponse
            {
                ApplicationId = app.Id,
                ClientId = app.ClientId,
                CategoryName = app.Category.Name,
                SubCategoryName = app.SubCategory.Name,
                Age = app.Age,
                Weight = app.Weight,
                Height = app.Height,
                LifePrice = app.LifePrice,
                LifeInsuranceType = app.LifeInsuranceType,
                Status = app.Status,
                CreatedAt = app.CreatedAt
            }).ToList();
        }

        public async Task<bool> ApproveMotorApplicationAsync(Guid applicationId)
        {
            var app = await _context.MotorInsuranceApplications
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .FirstOrDefaultAsync(a => a.Id == applicationId);


            if (app == null) return false;

            if (app.CalculatedPremium == 0)
                app.CalculatedPremium = app.MarketPrice * 0.025m;

            app.Status = "Approved";
            await _context.SaveChangesAsync();
            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <title>Motor Insurance Approved</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
  <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
    <h2 style='color: #2E86C1; text-align: center;'>Motor Insurance Application Approved</h2>
    <p>Dear <strong>{app.Client.FirstName}</strong>,</p>
    <p>We are pleased to inform you that your motor insurance application (ID: <strong>{app.Id}</strong>) has been <span style='color: green; font-weight: bold;'>approved</span>.</p>
    
    <h3 style='color: #2E86C1;'>Application Details</h3>
    <table style='width: 100%; border-collapse: collapse;'>
      <tr>
        <td style='padding: 8px; border: 1px solid #ddd;'>Category</td>
        <td style='padding: 8px; border: 1px solid #ddd;'>{app.Category.Name}</td>
      </tr>
      <tr>
        <td style='padding: 8px; border: 1px solid #ddd;'>Subcategory</td>
        <td style='padding: 8px; border: 1px solid #ddd;'>{app.SubCategory.Name}</td>
      </tr>
      <tr>
        <td style='padding: 8px; border: 1px solid #ddd;'>Model</td>
        <td style='padding: 8px; border: 1px solid #ddd;'>{app.Model}</td>
      </tr>
      <tr>
        <td style='padding: 8px; border: 1px solid #ddd;'>Plate Number</td>
        <td style='padding: 8px; border: 1px solid #ddd;'>{app.PlateNumber}</td>
      </tr>
      <tr>
        <td style='padding: 8px; border: 1px solid #ddd;'>Premium</td>
        <td style='padding: 8px; border: 1px solid #ddd;'>{app.CalculatedPremium:C}</td>
      </tr>
    </table>

    <p style='margin-top: 20px;'>You can now proceed with the payment to complete your insurance process.</p>

    <p style='margin-top: 30px;'>Thank you for choosing our services!<br />
    <strong>Insurance Company Name</strong></p>
  </div>
</body>
</html>
";

            await _emailService.SendEmailAsync(app.Client.User.Email,
                "Motor Insurance Application Approved", emailBody);

            return true;
        }

        public async Task<bool> ApproveLifeApplicationAsync(Guid applicationId)
        {
            var app = await _context.LifeInsuranceApplications
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Include(a => a.Beneficiaries)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (app == null) return false;
            if (app.LifePrice == 0) return false;

            app.Status = "Approved";
            await _context.SaveChangesAsync();

            // Build beneficiary table rows
            string beneficiaryRows = "";
            foreach (var b in app.Beneficiaries)
            {
                beneficiaryRows += $@"
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd;'>{b.Name}</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{b.Relation}</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{b.PhoneNumber}</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{b.Email}</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{app.SecretKey}</td>
        </tr>";
            }

            // Prepare email template
            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Life Insurance Application Approved</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #2E86C1; text-align: center;'>Life Insurance Application Approved</h2>

        <p>Dear <strong>{app.Client.FirstName}</strong>,</p>
        <p>We are pleased to inform you that your life insurance application (ID: <strong>{app.Id}</strong>) has been <span style='color: green; font-weight: bold;'>approved</span>.</p>

        <h3 style='color: #2E86C1;'>Application Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Category</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.Category.Name}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Subcategory</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.SubCategory.Name}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Age</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.Age}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Life Insurance Type</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.LifeInsuranceType}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Premium</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.LifePrice:C}</td>
            </tr>
        </table>

        <h3 style='color: #2E86C1; margin-top: 20px;'>Beneficiaries</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <thead>
                <tr>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Name</th>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Relation</th>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Phone</th>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Email</th>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Secret Key</th>
                </tr>
            </thead>
            <tbody>
                {beneficiaryRows}
            </tbody>
        </table>

        <p style='margin-top: 20px;'>You can now proceed with the payment to complete your insurance process.</p>

        <p style='margin-top: 30px;'>Thank you for choosing <strong>Your Insurance Company Name</strong>!</p>
    </div>
</body>
</html>";

            await _emailService.SendEmailAsync(
                app.Client.User.Email,
                "Life Insurance Application Approved",
                emailBody
            );

            return true;
        }

        public async Task<bool> RejectMotorApplicationAsync(Guid applicationId, string message)
        {
            var app = await _context.MotorInsuranceApplications
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (app == null) return false;

            app.Status = "Rejected";
            app.Message = message;
            await _context.SaveChangesAsync();

            // Email template
            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Motor Insurance Application Rejected</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #E74C3C; text-align: center;'>Motor Insurance Application Rejected</h2>

        <p>Dear <strong>{app.Client.FirstName}</strong>,</p>
        <p>We regret to inform you that your motor insurance application (ID: <strong>{app.Id}</strong>) has been <span style='color: red; font-weight: bold;'>rejected</span>.</p>
        
        <h3 style='color: #2E86C1;'>Application Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Category</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.Category.Name}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Subcategory</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.SubCategory.Name}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Model</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.Model}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Plate Number</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.PlateNumber}</td>
            </tr>
        </table>

        <p style='margin-top: 20px;'><strong>Reason for rejection:</strong> {message}</p>
        <p style='margin-top: 30px;'>If you have questions, please contact our support team.<br/>
        Thank you for considering <strong>Your Insurance Company Name</strong>.</p>
    </div>
</body>
</html>
";

            await _emailService.SendEmailAsync(app.Client.User.Email,
                "Motor Insurance Application Rejected", emailBody);

            return true;
        }

        public async Task<bool> RejectLifeApplicationAsync(Guid applicationId, string message)
        {
            var app = await _context.LifeInsuranceApplications
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Include(a => a.Beneficiaries)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (app == null) return false;

            app.Status = "Rejected";
            app.Message = message;
            await _context.SaveChangesAsync();

            // Build beneficiary rows dynamically
            string beneficiaryRows = "";
            foreach (var b in app.Beneficiaries)
            {
                beneficiaryRows += $@"
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd;'>{b.Name}</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{b.Relation}</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{b.PhoneNumber}</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{b.Email}</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{app.SecretKey}</td>
        </tr>";
            }

            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Life Insurance Application Rejected</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #E74C3C; text-align: center;'>Life Insurance Application Rejected</h2>

        <p>Dear <strong>{app.Client.FirstName}</strong>,</p>
        <p>We regret to inform you that your life insurance application (ID: <strong>{app.Id}</strong>) has been <span style='color: red; font-weight: bold;'>rejected</span>.</p>

        <h3 style='color: #2E86C1;'>Application Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Category</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.Category.Name}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Subcategory</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.SubCategory.Name}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Age</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.Age}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Life Insurance Type</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.LifeInsuranceType}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Premium</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{app.LifePrice:C}</td>
            </tr>
        </table>

        <h3 style='color: #2E86C1; margin-top: 20px;'>Beneficiaries</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <thead>
                <tr>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Name</th>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Relation</th>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Phone</th>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Email</th>
                    <th style='padding: 8px; border: 1px solid #ddd;'>Secret Key</th>
                </tr>
            </thead>
            <tbody>
                {beneficiaryRows}
            </tbody>
        </table>

        <p style='margin-top: 20px;'><strong>Reason for rejection:</strong> {message}</p>
        <p style='margin-top: 30px;'>If you have any questions, please contact our support team.<br/>
        Thank you for considering <strong>Your Insurance Company Name</strong>.</p>
    </div>
</body>
</html>
";

            await _emailService.SendEmailAsync(
                app.Client.User.Email,
                "Life Insurance Application Rejected",
                emailBody
            );

            return true;
        }

        public async Task<List<MotorInsuranceApplicationResponse>> GetApprovedMotorApplicationsAsync()
        {
            var applications = await _context.MotorInsuranceApplications
                .Where(a => a.Status == "Approved")
                .Include(a => a.Client)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .ToListAsync();

            return applications.Select(a => new MotorInsuranceApplicationResponse
            {
                ApplicationId = a.Id,
                ClientId = a.ClientId,
                CategoryName = a.Category?.Name ?? string.Empty,
                SubCategoryName = a.SubCategory?.Name ?? string.Empty,
                Model = a.Model,
                PlateNumber = a.PlateNumber,
                YearOfManufacture = a.YearOfManufacture,
                EngineNumber = a.EngineNumber,
                ChassisNumber = a.ChassisNumber,
                MarketPrice = a.MarketPrice,
                CalculatedPremium = a.CalculatedPremium,
                InsuranceType = a.InsuranceType,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                Message = "Your insurance application has been approved."
            }).ToList();
        }

        public async Task<List<LifeInsuranceApplicationResponse>> GetApprovedLifeApplicationsAsync()
        {
            var applications = await _context.LifeInsuranceApplications
                .Where(a => a.Status == "Approved")
                .Include(a => a.Client)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .ToListAsync();

            return applications.Select(a => new LifeInsuranceApplicationResponse
            {
                ApplicationId = a.Id,
                ClientId = a.ClientId,
                CategoryName = a.Category?.Name ?? string.Empty,
                SubCategoryName = a.SubCategory?.Name ?? string.Empty,
                Age = a.Age,
                Weight = a.Weight,
                Height = a.Height,

                LifePrice = a.LifePrice,
                LifeInsuranceType = a.LifeInsuranceType,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                Message = "Your insurance application has been approved."
            }).ToList();
        }

        public async Task<List<MotorInsuranceApplicationResponse>> GetRejectedMotorApplicationsAsync()
        {
            var applications = await _context.MotorInsuranceApplications
                .Where(a => a.Status == "Rejected")
                .Include(a => a.Client)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .ToListAsync();

            return applications.Select(a => new MotorInsuranceApplicationResponse
            {
                ApplicationId = a.Id,
                ClientId = a.ClientId,
                CategoryName = a.Category?.Name ?? string.Empty,
                SubCategoryName = a.SubCategory?.Name ?? string.Empty,
                Model = a.Model,
                PlateNumber = a.PlateNumber,
                YearOfManufacture = a.YearOfManufacture,
                EngineNumber = a.EngineNumber,
                ChassisNumber = a.ChassisNumber,
                MarketPrice = a.MarketPrice,
                CalculatedPremium = a.CalculatedPremium,
                InsuranceType = a.InsuranceType,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                Message = "Your insurance application was rejected."
            }).ToList();
        }

        public async Task<List<LifeInsuranceApplicationResponse>> GetRejectedLifeApplicationsAsync()
        {
            var applications = await _context.LifeInsuranceApplications
                .Where(a => a.Status == "Rejected")
                .Include(a => a.Client)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .ToListAsync();

            return applications.Select(a => new LifeInsuranceApplicationResponse
            {
                ApplicationId = a.Id,
                ClientId = a.ClientId,
                CategoryName = a.Category?.Name ?? string.Empty,
                SubCategoryName = a.SubCategory?.Name ?? string.Empty,
                Age = a.Age,
                Weight = a.Weight,
                Height = a.Height,

                LifePrice = a.LifePrice,
                LifeInsuranceType = a.LifeInsuranceType,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                Message = "Your insurance application has been approved."
            }).ToList();
        }

        public async Task<FinanceResponse?> GetMyProfileAsync(Guid userId)
        {
            var client = await _context.Finances
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null) return null;

            return new FinanceResponse
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

        public async Task<List<ApproveClaimResponse>> GetAllApprovedByManagerClaimsAsync()
        {
            var claims = await _context.Claims
                .Include(c => c.Client)
                .ThenInclude(cl => cl.User)
                .Include(c => c.Operator)
                .Where(c => c.Status == ClaimStatus.ApprovedByManager)
                .OrderByDescending(c => c.ApprovedByManagerAt)
                .ToListAsync();

            return claims.Select(c => new ApproveClaimResponse
            {
                ClientFullName = $"{c.Client.FirstName} {c.Client.FatherName} {c.Client.GrandFatherName}",
                ClaimId = c.Id,
                Status = c.Status,
                ApprovedAmount = c.ApprovedAmount ?? 0,
                ApprovedAt = c.ApprovedByManagerAt ?? DateTime.MinValue,
            }).ToList();
        }

        public async Task<FinanceDashboardResponse> GetFinanceDashboardAsync()
        {
            // Motor counts
            var totalMotor = await _context.MotorInsuranceApplications.CountAsync();
            var pendingMotor = await _context.MotorInsuranceApplications
                .CountAsync(a => a.Status == "Pending");
            var approvedMotor = await _context.MotorInsuranceApplications
                .CountAsync(a => a.Status == "Approved");
            var rejectedMotor = await _context.MotorInsuranceApplications
                .CountAsync(a => a.Status == "Rejected");

            // Life counts
            var totalLife = await _context.LifeInsuranceApplications.CountAsync();
            var pendingLife = await _context.LifeInsuranceApplications
                .CountAsync(a => a.Status == "Pending");
            var approvedLife = await _context.LifeInsuranceApplications
                .CountAsync(a => a.Status == "Approved");
            var rejectedLife = await _context.LifeInsuranceApplications
                .CountAsync(a => a.Status == "Rejected");

            return new FinanceDashboardResponse
            {
                TotalApplications = totalMotor + totalLife,

                PendingApplications = pendingMotor + pendingLife,
                ApprovedApplications = approvedMotor + approvedLife,
                RejectedApplications = rejectedMotor + rejectedLife,

                MotorApplications = totalMotor,
                LifeApplications = totalLife
            };
        }

    }
}
