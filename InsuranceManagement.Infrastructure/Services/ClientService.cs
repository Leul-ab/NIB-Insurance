using InsuranceManagement.Application.DTO.Requests;
using InsuranceManagement.Application.DTO.Responses;
using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Domain.Enums;
using InsuranceManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace InsuranceManagement.Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private readonly InsuranceDbContext _context;
        private readonly IEmailService _emailService;

        public ClientService(InsuranceDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }
        public async Task<ClientResponse> RegisterClientAsync(ClientRegisterRequest request)
        {
            // Check email duplication
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("User with this email already exists.");

            if (await _context.Clients.AnyAsync(c => c.User.Email == request.Email))
                throw new Exception("Client with this email already exists.");

            // Create User
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.Email,
                Role = UserRole.Client,
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            string passportImagePath = await SaveFileAsync(request.PassportOrNationalIdImage, "ClientDocs");


            // Create Client
            var client = new Client
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                FatherName = request.FatherName,
                GrandFatherName = request.GrandFatherName,
                DateOfBirth = request.DateOfBirth,

                PhoneNumber = request.PhoneNumber,
                NationalIdOrPassport = request.NationalIdOrPassport,
                Gender = request.Gender,

                Region = request.Region,
                City = request.City,
                SubCity = request.SubCity,

                PassportOrNationalIdImageUrl = passportImagePath,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,

                UserId = user.Id,
                User = user
            };

            _context.Users.Add(user);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return new ClientResponse
            {
                Id = client.Id,
                FirstName = client.FirstName,
                FatherName = client.FatherName,
                GrandFatherName = client.GrandFatherName,
                DateOfBirth = client.DateOfBirth,
                Email = client.User.Email,
                PhoneNumber = client.PhoneNumber,
                NationalIdOrPassport = client.NationalIdOrPassport,
                Gender = (Gender)client.Gender,


                PassportOrNationalIdImageUrl = client.PassportOrNationalIdImageUrl,

                Region = client.Region,
                City = client.City,
                SubCity = client.SubCity,
                //LogoImageUrl = client.LogoImageUrl,

                CreatedAt = client.CreatedAt,
                //IsActive = client.IsActive
            };
        }

        public async Task<ClientResponse> GetClientByIdAsync(Guid id)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return null;

            return new ClientResponse
            {
                Id = client.Id,
                Email = client.User.Email,
                FirstName = client.FirstName,
                FatherName = client.FatherName,
                GrandFatherName = client.GrandFatherName,
                DateOfBirth = client.DateOfBirth,
                PhoneNumber = client.PhoneNumber,
                NationalIdOrPassport = client.NationalIdOrPassport,
                Gender = (Gender)client.Gender,
                Region = client.Region,
                City = client.City,
                SubCity = client.SubCity,
                PassportOrNationalIdImageUrl = client.PassportOrNationalIdImageUrl,
                CreatedAt = client.CreatedAt,
                //IsActive = client.IsActive
            };
        }

        public async Task<ClientResponse> GetClientByUserIdAsync(Guid userId)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null) return null;

            return new ClientResponse
            {
                Id = client.Id,
                FirstName = client.FirstName,
                FatherName = client.FatherName,
                GrandFatherName = client.GrandFatherName,
                DateOfBirth = client.DateOfBirth,
                Gender = (Gender)client.Gender,
                Email = client.User?.Email ?? string.Empty,
                PhoneNumber = client.PhoneNumber,
                Region = client.Region,
                City = client.City,
                SubCity = client.SubCity,
                NationalIdOrPassport = client.NationalIdOrPassport,
                PassportOrNationalIdImageUrl = client.PassportOrNationalIdImageUrl,
                CreatedAt = client.CreatedAt
            };
        }

        public async Task<List<ClientResponse>> GetAllClientsAsync()
        {
            var clients = await _context.Clients
                .Include(c => c.User)
                .ToListAsync();

            return clients.Select(client => new ClientResponse
            {
                Id = client.Id,
                Email = client.User.Email,
                FirstName = client.FirstName,
                FatherName = client.FatherName,
                GrandFatherName = client.GrandFatherName,
                DateOfBirth = client.DateOfBirth,
                PhoneNumber = client.PhoneNumber,
                NationalIdOrPassport = client.NationalIdOrPassport,
                Gender = (Gender)client.Gender,
                Region = client.Region,
                City = client.City,
                SubCity = client.SubCity,
                PassportOrNationalIdImageUrl = client.PassportOrNationalIdImageUrl,
                CreatedAt = client.CreatedAt,
                //IsActive = client.IsActive
            }).ToList();
        }

        public async Task<ClientResponse> UpdateClientAsync(Guid clientId, ClientUpdateRequest request)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            if (client == null)
                throw new Exception("Client not found.");

            // ===== PERSONAL INFO =====
            if (!string.IsNullOrEmpty(request.FirstName))
                client.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.FatherName))
                client.FatherName = request.FatherName;

            if (!string.IsNullOrEmpty(request.GrandFatherName))
                client.GrandFatherName = request.GrandFatherName;

            if (request.DateOfBirth.HasValue)
            {
                var age = DateTime.Now.Year - request.DateOfBirth.Value.Year;
                if (age < 18)
                    throw new Exception("Client must be at least 18 years old.");

                client.DateOfBirth = request.DateOfBirth.Value;
            }

            if (request.Gender.HasValue)
                client.Gender = request.Gender;

            if (!string.IsNullOrEmpty(request.NationalIdOrPassport))
                client.NationalIdOrPassport = request.NationalIdOrPassport;

            // ===== CONTACT INFO =====
            if (!string.IsNullOrEmpty(request.Email) && client.User != null)
                client.User.Email = request.Email;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                client.PhoneNumber = request.PhoneNumber;

            // ===== ADDRESS =====
            if (!string.IsNullOrEmpty(request.Region))
                client.Region = request.Region;

            if (!string.IsNullOrEmpty(request.City))
                client.City = request.City;

            if (!string.IsNullOrEmpty(request.SubCity))
                client.SubCity = request.SubCity;


            // ===== PASSPORT / NATIONAL ID IMAGE UPLOAD =====
            if (request.PassportOrNationalIdImage != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(client.PassportOrNationalIdImageUrl))
                {
                    var oldPath = Path.Combine("wwwroot", client.PassportOrNationalIdImageUrl.TrimStart('/'));
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                // Save new image
                var newImagePath = await SaveFileAsync(request.PassportOrNationalIdImage, "ClientDocs");
                client.PassportOrNationalIdImageUrl = newImagePath;
            }


            await _context.SaveChangesAsync();
            // ✅ Send email after update
            var emailSubject = "Profile Updated Successfully";
            var emailBody = $@"
        Hello {client.FirstName},<br/><br/>
        Your Client profile has been updated successfully.<br/>
        If you did not make this change, please contact support immediately.<br/><br/>
        Regards,<br/>
        NIB Team";

            await _emailService.SendEmailAsync(client.User.Email, emailSubject, emailBody);

            return new ClientResponse
            {
                Id = client.Id,

                // Personal
                FirstName = client.FirstName,
                FatherName = client.FatherName,
                GrandFatherName = client.GrandFatherName,
                Gender = (Gender)client.Gender,
                DateOfBirth = client.DateOfBirth,
                NationalIdOrPassport = client.NationalIdOrPassport,

                // Contact
                Email = client.User?.Email ?? string.Empty,
                PhoneNumber = client.PhoneNumber,

                // Address
                Region = client.Region,
                City = client.City,
                SubCity = client.SubCity,

                // NEW — return image URL
                PassportOrNationalIdImageUrl = client.PassportOrNationalIdImageUrl,

                CreatedAt = client.CreatedAt,
            };
        }

        public async Task<bool> DeleteClientAsync(Guid id)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null) return false;

            // Remove associated User first
            if (client.User != null)
                _context.Users.Remove(client.User);

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }






        

        public async Task<bool> SendPasswordResetOtpAsync(ForgotPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return false;

            var otp = new Random().Next(100000, 999999).ToString();
            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5); // expires in 5 min
            user.IsOtpVerified = false; // reset verification status

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(user.Email, "Password Reset OTP",
                $"Your OTP code is {otp}. It will expire in 5 minutes.");

            return true;
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return false;

            if (user.OtpCode == request.OtpCode && user.OtpExpiry.HasValue && user.OtpExpiry > DateTime.UtcNow)
            {
                user.IsOtpVerified = true; // mark OTP as verified
                await _context.SaveChangesAsync();
                return true;
            }

            return false; // invalid or expired OTP
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return false;

            // Only allow password reset if OTP was verified
            if (!user.IsOtpVerified) return false;

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);

            // Invalidate OTP and verification status
            user.OtpCode = null;
            user.OtpExpiry = null;
            user.IsOtpVerified = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResendOtpAsync(ResendOtpRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return false;

            // Generate new OTP
            var otp = new Random().Next(100000, 999999).ToString();

            user.OtpCode = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5); // reset expiry

            await _context.SaveChangesAsync();

            // Send OTP email
            await _emailService.SendEmailAsync(user.Email, "Password Reset OTP - Resend",
                $"Your new OTP code is {otp}. It will expire in 5 minutes.");

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

        public async Task<ClientResponse?> GetMyProfileAsync(Guid userId)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null) return null;

            return new ClientResponse
            {
                Id = client.Id,
                FirstName = client.FirstName,
                FatherName = client.FatherName,
                GrandFatherName = client.GrandFatherName,
                DateOfBirth = client.DateOfBirth,
                Gender = (Gender)client.Gender,
                NationalIdOrPassport = client.NationalIdOrPassport,
                PassportOrNationalIdImageUrl = client.PassportOrNationalIdImageUrl,
                LogoImageUrl = client.LogoImageUrl,
                Email = client.User?.Email ?? string.Empty,
                PhoneNumber = client.PhoneNumber,
                Region = client.Region,
                City = client.City,
                SubCity = client.SubCity,
                CreatedAt = client.CreatedAt
            };
        }

        public async Task<MotorInsuranceApplicationResponse> PreviewMotorInsuranceAsync(Guid clientId, MotorInsuranceApplyRequest request)
        {
            var client = await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null)
                throw new Exception("Client not found.");

            var category = await _context.OperatorCategories
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.ParentId == null);
            if (category == null)
                throw new Exception("Invalid main category.");

            var subcategory = await _context.OperatorCategories
                .FirstOrDefaultAsync(c => c.Id == request.SubCategoryId && c.ParentId == request.CategoryId);
            if (subcategory == null)
                throw new Exception("Invalid subcategory.");

            var marketPrice = GetMarketPriceByYear(request.YearOfManufacture);
            decimal percentage = request.InsuranceType switch
            {
                InsuranceType.Full => subcategory.FullInsurancePercentage ?? 2m,
                InsuranceType.ThirdParty => subcategory.ThirdPartyPercentage ?? 1m,
                _ => 2m
            };

            var calculatedPremium = Decimal.Round(marketPrice * (percentage / 100m), 2);

            // Save images temporarily
            string carImagePath = await SaveFileAsync(request.CarImage, "CarImages");
            string libreImagePath = await SaveFileAsync(request.CarLibreImage, "CarLibreImages");

            // Create draft application
            var application = new MotorInsuranceApplication
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                CategoryId = request.CategoryId,
                SubCategoryId = request.SubCategoryId,
                Model = request.Model,
                PlateNumber = request.PlateNumber,
                YearOfManufacture = request.YearOfManufacture,
                EngineNumber = request.EngineNumber,
                ChassisNumber = request.ChassisNumber,
                MarketPrice = marketPrice,
                CalculatedPremium = calculatedPremium,
                InsuranceType = request.InsuranceType,
                Status = "Draft",
                CreatedAt = DateTime.UtcNow,
                CarImagePath = carImagePath,
                CarLibreImagePath = libreImagePath
            };

            _context.MotorInsuranceApplications.Add(application);
            await _context.SaveChangesAsync();

            return new MotorInsuranceApplicationResponse
            {
                ApplicationId = application.Id,
                ClientId = clientId,
                CategoryName = application.Category.Name,
                SubCategoryName = application.SubCategory.Name,
                Model = application.Model,
                PlateNumber = application.PlateNumber,
                YearOfManufacture = application.YearOfManufacture,
                MarketPrice = application.MarketPrice,
                ChassisNumber = application.ChassisNumber,
                EngineNumber = application.EngineNumber,
                CalculatedPremium = application.CalculatedPremium,
                InsuranceType = application.InsuranceType,
                Status = application.Status,
                CreatedAt = application.CreatedAt,
                Message = "Please review and confirm this application.",
                CarImageUrl = application.CarImagePath,
                CarLibreImageUrl = application.CarLibreImagePath,
                ClientFullName = $"{application.Client.FirstName} {application.Client.FatherName} {application.Client.GrandFatherName}",
                ClientNationalIdOrPassport = application.Client.NationalIdOrPassport,
                ClientPassportOrNationalIdImageUrl = application.Client.PassportOrNationalIdImageUrl,
                ClientEmail = application.Client.User.Email,
                ClientPhoneNumber = application.Client.PhoneNumber,
                ClientDateOfBirth = application.Client.DateOfBirth,
                ClientGender = (Gender)application.Client.Gender
            };
            
        }

        public async Task<MotorInsuranceApplicationResponse> ConfirmMotorInsuranceAsync(Guid clientId, MotorInsuranceConfirmRequest request)
        {
            var application = await _context.MotorInsuranceApplications
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .FirstOrDefaultAsync(a => a.Id == request.ApplicationId && a.ClientId == clientId);

            if (application == null)
                throw new Exception("Application not found.");

            if (application.Status != "Draft")
                throw new Exception("This application has already been submitted.");

            // Change status
            application.Status = "Pending";
            application.CreatedAt = DateTime.UtcNow;

            _context.MotorInsuranceApplications.Update(application);
            await _context.SaveChangesAsync();

            // Send email using your existing EmailService
            var emailBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <title>Motor Insurance Application Submitted</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
    <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
        <h2 style='color: #2E86C1; text-align: center;'>Motor Insurance Application Submitted</h2>
        
        <p>Dear <strong>{application.Client.FirstName}</strong>,</p>
        <p>Your motor insurance application (ID: <strong>{application.Id}</strong>) has been successfully submitted. Our finance team will review it shortly.</p>

        <h3 style='color: #2E86C1;'>Application Details</h3>
        <table style='width: 100%; border-collapse: collapse;'>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Category</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{application.Category.Name}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Subcategory</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{application.SubCategory.Name}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Model</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{application.Model}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Plate Number</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{application.PlateNumber}</td>
            </tr>
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>Premium</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{application.CalculatedPremium:C}</td>
            </tr>
        </table>

        <p style='margin-top: 20px;'>You will be notified once your application is processed.</p>

        <p style='margin-top: 30px;'>Thank you for choosing <strong>- NIB Insurance</strong>!</p>
    </div>
</body>
</html>
";


            await _emailService.SendEmailAsync(application.Client.User.Email,
                "Motor Insurance Application Confirmed", emailBody);


            return new MotorInsuranceApplicationResponse
            {
                ApplicationId = application.Id,
                ClientId = clientId,
                CategoryName = application.Category.Name,
                SubCategoryName = application.SubCategory.Name,
                Model = application.Model,
                PlateNumber = application.PlateNumber,
                YearOfManufacture = application.YearOfManufacture,
                MarketPrice = application.MarketPrice,
                ChassisNumber = application.ChassisNumber,
                EngineNumber = application.EngineNumber,
                CalculatedPremium = application.CalculatedPremium,
                InsuranceType = application.InsuranceType,
                Status = application.Status,
                CreatedAt = application.CreatedAt,
                Message = "Your application has been submitted to finance.",
                CarImageUrl = application.CarImagePath,
                CarLibreImageUrl = application.CarLibreImagePath,
                ClientFullName = $"{application.Client.FirstName} {application.Client.FatherName} {application.Client.GrandFatherName}",
                ClientNationalIdOrPassport = application.Client.NationalIdOrPassport,
                ClientPassportOrNationalIdImageUrl = application.Client.PassportOrNationalIdImageUrl,
                ClientEmail = application.Client.User.Email,
                ClientPhoneNumber = application.Client.PhoneNumber,
                ClientDateOfBirth = application.Client.DateOfBirth,
                ClientGender = (Gender)application.Client.Gender
            };
        }

        public async Task<LifeInsuranceApplicationResponse> PreviewLifeInsuranceAsync(Guid clientId, LifeInsuranceApplyRequest request)
        {
            var client = await _context.Clients.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null) throw new Exception("Client not found.");

            var category = await _context.OperatorCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.ParentId == null);
            if (category == null) throw new Exception("Invalid main category.");

            var subcategory = await _context.OperatorCategories.FirstOrDefaultAsync(c => c.Id == request.SubCategoryId && c.ParentId == request.CategoryId);
            if (subcategory == null) throw new Exception("Invalid subcategory.");

            decimal lifePrice = request.LifeInsuranceType switch
            {
                LifeInsuranceType.FullLife => subcategory.FullLifePrice ?? 0m,
                LifeInsuranceType.HalfLife => subcategory.HalfLifePrice ?? 0m,
                _ => 0m
            };




            // Create application
            var application = new LifeInsuranceApplication
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                CategoryId = request.CategoryId,
                SubCategoryId = request.SubCategoryId,
                Age = request.Age,
                Height = request.Height,
                Weight = request.Weight,
                LifePrice = lifePrice,
                LifeInsuranceType = request.LifeInsuranceType,
                Status = "Draft",
                Message = request.Message,
                CreatedAt = DateTime.UtcNow
            };

            _context.LifeInsuranceApplications.Add(application);

            



            await _context.SaveChangesAsync();

            return new LifeInsuranceApplicationResponse
            {
                ApplicationId = application.Id,
                ClientId = clientId,
                CategoryName = category.Name,
                SubCategoryName = subcategory.Name,
                Age = application.Age,
                Height = application.Height,
                Weight = application.Weight,
                LifePrice = application.LifePrice,
                LifeInsuranceType = application.LifeInsuranceType,
                Status = application.Status,
                CreatedAt = application.CreatedAt,
                Message = "Please review and confirm this application.",
                ClientFullName = $"{client.FirstName} {client.FatherName} {client.GrandFatherName}",
                ClientEmail = client.User.Email,
                ClientPhoneNumber = client.PhoneNumber,
                ClientGender = (Gender)client.Gender,
                ClientDateOfBirth = client.DateOfBirth,
                ClientNationalIdOrPassport = client.NationalIdOrPassport,
                ClientPassportOrNationalIdImageUrl = client.PassportOrNationalIdImageUrl,
                Beneficiaries = new List<BeneficiaryResponse>()
            };
        }

        public async Task<BeneficiaryResponse> AddSingleBeneficiaryLifeInsuranceAsync(Guid applicationId, BeneficiaryRequest beneficiary)
        {
            if (beneficiary == null)
                throw new Exception("Beneficiary is required.");

            // Load and track the application entity
            var application = await _context.LifeInsuranceApplications
                .Include(a => a.Beneficiaries)
                .FirstOrDefaultAsync(a => a.Id == applicationId);

            if (application == null)
                throw new Exception("Application not found.");

            if (application.Beneficiaries.Count >= 3)
                throw new Exception("Maximum 3 beneficiaries allowed.");

            string? filePath = null;
            if (beneficiary.NationalIdFile != null)
            {
                filePath = await SaveFileAsync(beneficiary.NationalIdFile, "BeneficiaryNationalIds");
            }

            var entity = new LifeInsuranceBeneficiary
            {
                Name = beneficiary.Name,
                Relation = beneficiary.Relation,
                PhoneNumber = beneficiary.PhoneNumber,
                Email = beneficiary.Email,
                NationalIdFilePath = filePath,
                LifeInsuranceApplicationId = application.Id // VERY IMPORTANT
            };

            // Add entity directly to the DbSet
            _context.Beneficiaries.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Optional: log ex
                throw new Exception("Failed to add beneficiary. The application may have been modified or deleted.", ex);
            }

            return new BeneficiaryResponse
            {
                Name = entity.Name,
                Relation = entity.Relation,
                PhoneNumber = entity.PhoneNumber,
                Email = entity.Email,
                NationalIdFilePath = entity.NationalIdFilePath
            };
        }

        public async Task<LifeInsuranceApplicationResponse> ConfirmLifeInsuranceAsync(Guid clientId, LifeInsuranceConfirmRequest request)
        {
            var application = await _context.LifeInsuranceApplications
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Include(a => a.Beneficiaries)
                .FirstOrDefaultAsync(a => a.Id == request.ApplicationId && a.ClientId == clientId);

            if (application == null)
                throw new Exception("Application not found.");

            if (application.Status != "Draft")
                throw new Exception("This application has already been submitted.");

            // 🆕 Generate NEW secret key for claim validation
            string secretKey = GenerateSecretKey();
            application.SecretKey = secretKey;

            application.Status = "Pending";
            application.CreatedAt = DateTime.UtcNow;

            _context.LifeInsuranceApplications.Update(application);
            await _context.SaveChangesAsync();

            // ===============================
            // EMAIL TO CLIENT
            // ===============================
            var clientEmailBody = EmailTemplateService.GenerateLifeSubmittedEmail(application, secretKey);

            await _emailService.SendEmailAsync(
                application.Client.User.Email,
                "Life Insurance Application Submitted",
                clientEmailBody
            );

            // ===============================
            // EMAIL TO ALL BENEFICIARIES
            // ===============================
            foreach (var b in application.Beneficiaries)
            {
                if (!string.IsNullOrWhiteSpace(b.Email))
                {
                    var beneficiaryEmailBody = EmailTemplateService.GenerateBeneficiaryAddedEmail(b, secretKey);

                    await _emailService.SendEmailAsync(
                        b.Email,
                        "You Have Been Added as a Beneficiary",
                        beneficiaryEmailBody
                    );
                }
            }

            // ===============================
            // RETURN UPDATED RESPONSE
            // ===============================
            return new LifeInsuranceApplicationResponse
            {
                ApplicationId = application.Id,
                ClientId = application.ClientId,

                CategoryName = application.Category.Name,
                SubCategoryName = application.SubCategory.Name,

                Age = application.Age,
                Height = application.Height,
                Weight = application.Weight,
                LifePrice = application.LifePrice,
                LifeInsuranceType = application.LifeInsuranceType,
                Status = application.Status,
                CreatedAt = application.CreatedAt,

                SecretKey = application.SecretKey,
                Message = "Your life insurance application has been submitted for financial review.",

                ClientFullName = $"{application.Client.FirstName} {application.Client.FatherName} {application.Client.GrandFatherName}",
                ClientEmail = application.Client.User.Email,
                ClientPhoneNumber = application.Client.PhoneNumber,
                ClientGender = (Gender)application.Client.Gender,
                ClientDateOfBirth = application.Client.DateOfBirth,
                ClientNationalIdOrPassport = application.Client.NationalIdOrPassport,
                ClientPassportOrNationalIdImageUrl = application.Client.PassportOrNationalIdImageUrl,

                Beneficiaries = application.Beneficiaries.Select(b => new BeneficiaryResponse
                {
                    Name = b.Name,
                    Relation = b.Relation,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    NationalIdFilePath = b.NationalIdFilePath
                }).ToList()
            };
        }

        public async Task<List<MotorInsuranceApplicationResponse>>GetClientMotorApplicationsAsync(Guid clientId)
        {
            var applications = await _context.MotorInsuranceApplications
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Where(a => a.ClientId == clientId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return applications.Select(app => new MotorInsuranceApplicationResponse
            {
                ApplicationId = app.Id,
                ClientId = app.ClientId,

                CategoryName = app.Category.Name,
                SubCategoryName = app.SubCategory.Name,

                Model = app.Model,
                PlateNumber = app.PlateNumber,
                YearOfManufacture = app.YearOfManufacture,
                EngineNumber = app.EngineNumber,
                ChassisNumber = app.ChassisNumber,
                MarketPrice = app.MarketPrice,
                CalculatedPremium = app.CalculatedPremium,
                InsuranceType = app.InsuranceType,
                Status = app.Status,

                CarImageUrl = app.CarImagePath,
                CarLibreImageUrl = app.CarLibreImagePath,
                CreatedAt = app.CreatedAt,

                Message = app.Status switch
                {
                    "Pending" => "Your application is being reviewed by finance officers.",
                    "Approved" => "Your insurance application has been approved.",
                    "Rejected" => "Your insurance application was rejected.",
                    _ => "Unknown status."
                }
            }).ToList();
        }

        public async Task<List<LifeInsuranceApplicationResponse>> GetClientLifeApplicationsAsync(Guid clientId)
        {
            var applications = await _context.LifeInsuranceApplications
                .Include(a => a.Category)
                .Include(a => a.SubCategory)
                .Where(a => a.ClientId == clientId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return applications.Select(app => new LifeInsuranceApplicationResponse
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

                CreatedAt = app.CreatedAt,

                Message = app.Status switch
                {
                    "Pending" => "Your application is being reviewed by finance officers.",
                    "Approved" => "Your insurance application has been approved.",
                    "Rejected" => "Your insurance application was rejected.",
                    _ => "Unknown status."
                }
            }).ToList();
        }




        private decimal GetMarketPriceByYear(int year)
        {
            // Choose ranges and prices you agreed on:
            if (year >= 2021) return 2500000m;
            if (year >= 2016) return 1800000m;
            if (year >= 2011) return 1200000m;
            if (year >= 2006) return 800000m;
            if (year >= 2000) return 500000m;
            if (year >= 1990) return 300000m;

            return 150000m; // older than 1990
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            string uploadsFolder = Path.Combine("wwwroot", folderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{uniqueFileName}";
        }

        private string GenerateSecretKey()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", "")
                .Replace("+", "")
                .Replace("/", "");
        }




        public async Task<ClientDashboardResponse> GetClientDashboardAsync(Guid clientId)
        {
            // Motor insurance stats
            var motorApps = await _context.MotorInsuranceApplications
                .Where(a => a.ClientId == clientId)
                .ToListAsync();

            // Life insurance stats
            var lifeApps = await _context.LifeInsuranceApplications
                .Where(a => a.ClientId == clientId)
                .ToListAsync();

            int totalApplications = motorApps.Count + lifeApps.Count;

            int approvedApplications =
                motorApps.Count(a => a.Status == "Approved") +
                lifeApps.Count(a => a.Status == "Approved");

            int motorPolicies = motorApps.Count(a => a.Status == "Approved");
            int lifePolicies = lifeApps.Count(a => a.Status == "Approved");

            decimal totalPaidAmount =
                motorApps.Where(a => a.IsPaid).Sum(a => a.CalculatedPremium) +
                lifeApps.Where(a => a.IsPaid).Sum(a => a.LifePrice);

            return new ClientDashboardResponse
            {
                TotalApplications = totalApplications,
                ApprovedApplications = approvedApplications,
                MotorPolicies = motorPolicies,
                LifePolicies = lifePolicies,
                TotalPaidAmount = totalPaidAmount
            };
        }

        public async Task<ClaimResponse> ReportMotorClaimAsync(Guid userId, Guid motorApplicationId, MotorClaimReportRequest request)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == userId);
            if (client == null) throw new Exception("Client not found.");

            var policy = await _context.MotorInsuranceApplications
                .FirstOrDefaultAsync(a => a.Id == motorApplicationId && a.ClientId == client.Id && a.IsPaid);
            if (policy == null) throw new Exception("Invalid or unpaid motor policy.");

            var imageUrls = new List<string>();
            if (request.EvidenceImages != null)
            {
                foreach (var image in request.EvidenceImages)
                    imageUrls.Add(await SaveFileAsync(image, "ClaimEvidence"));
            }

            //var pendingClaims = await _context.Claims
            //    .CountAsync(c => c.OperatorId == operatorId &&
            //                     c.Status == ClaimStatus.Pending);

            string pdfPath = null;
            if (request.TrafficPoliceReportPdf != null)
                pdfPath = await SaveFileAsync(request.TrafficPoliceReportPdf, "TrafficReports");

            var claim = new ClaimR
            {
                Id = Guid.NewGuid(),
                ClientId = client.Id,
                MotorInsuranceApplicationId = motorApplicationId,
                IncidentDate = request.IncidentDate,
                IncidentTime = request.IncidentTime,
                Location = request.Location,
                IncidentType = request.IncidentType,
                Description = request.Description,
                EvidenceImagesJson = JsonSerializer.Serialize(imageUrls),
                TrafficPoliceReportPdfUrl = pdfPath,
                Status = ClaimStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return new ClaimResponse
            {
                ClaimId = claim.Id,
                ClientId = client.Id,
                MotorInsuranceApplicationId = motorApplicationId,
                IncidentDate = claim.IncidentDate,
                IncidentTime = claim.IncidentTime,
                Location = claim.Location,
                IncidentType = claim.IncidentType,
                Description = claim.Description,
                EvidenceImageUrls = imageUrls,
                TrafficPoliceReportPdfUrl = pdfPath,
                Status = claim.Status,
                CreatedAt = claim.CreatedAt,
                ApprovedAmount = claim.ApprovedAmount,
                RejectionReason = claim.RejectionReason,
                ClientEmail = claim.ClientEmail,
                ClientFirstName = claim.ClientFirstName,
                ClientFatherName = claim.ClientFatherName,
                ClientGrandFatherName = claim.ClientGrandFatherName,
            };
        }

        public async Task<ClaimResponse> ReportLifeClaimAsync(Guid applicationId, LifeClaimReportRequest request)
        {
            // Verify policy
            var policy = await _context.LifeInsuranceApplications
                .Include(a => a.Beneficiaries)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.IsPaid)
                ?? throw new Exception("Invalid or unpaid life policy.");

            // Get insured client
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.Id == policy.ClientId)
                ?? throw new Exception("Insured client not found.");

            // Create claim
            var claim = new ClaimR
            {
                Id = Guid.NewGuid(),
                ClientId = client.Id,
                LifeInsuranceApplicationId = applicationId,

                IncidentDate = request.IncidentDate,
                IncidentTime = TimeSpan.Zero,
                Location = "N/A",

                ClaimReason = request.ClaimReason,
                DeathCertificatePdf = request.DeathCertificatePdf != null
                    ? await SaveFileAsync(request.DeathCertificatePdf, "LifeClaims")
                    : null,
                MedicalReportPdf = request.MedicalReportPdf != null
                    ? await SaveFileAsync(request.MedicalReportPdf, "LifeClaims")
                    : null,
                HospitalDischargeSummaryPdf = request.HospitalDischargeSummaryPdf != null
                    ? await SaveFileAsync(request.HospitalDischargeSummaryPdf, "LifeClaims")
                    : null,

                HospitalName = request.HospitalName,
                Description = request.Description,

                // Motor fields reset
                EvidenceImagesJson = null,
                TrafficPoliceReportPdfUrl = null,
                MotorInsuranceApplicationId = null,

                Status = ClaimStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return new ClaimResponse
            {
                ClaimId = claim.Id,
                ClientId = client.Id,
                LifeInsuranceApplicationId = applicationId,
                IncidentDate = claim.IncidentDate,
                ClaimReason = claim.ClaimReason,
                DeathCertificatePdf = claim.DeathCertificatePdf,
                MedicalReportPdf = claim.MedicalReportPdf,
                HospitalDischargeSummaryPdf = claim.HospitalDischargeSummaryPdf,
                HospitalName = claim.HospitalName,
                Description = claim.Description,
                Status = claim.Status,
                CreatedAt = claim.CreatedAt,
                ClientFirstName = claim.ClientFirstName,
                ClientFatherName = claim.ClientFatherName,
                ClientGrandFatherName = claim.ClientGrandFatherName,
                ClientEmail = claim.ClientEmail,
                ApprovedAmount = claim.ApprovedAmount,
                Beneficiaries = policy.Beneficiaries.Select(b => new BeneficiaryResponse
                {
                    Name = b.Name,
                    Relation = b.Relation,
                    PhoneNumber = b.PhoneNumber,
                    Email = b.Email,
                    NationalIdFilePath = b.NationalIdFilePath
                }).ToList()

            };
        }

        public async Task<VerifyLifePolicyResponse> VerifyLifePolicyAsync(string secretKey, string beneficiaryEmail)
        {
            var policy = await _context.LifeInsuranceApplications
                .Include(a => a.Client).ThenInclude(c => c.User)
                .Include(a => a.Beneficiaries) // load multiple beneficiaries
                .FirstOrDefaultAsync(a => a.SecretKey == secretKey);

            if (policy == null)
                throw new Exception("Invalid policy secret key.");

            if (!policy.IsPaid)
                throw new Exception("This policy is not active or unpaid.");

            // beneficiary check (case insensitive)
            var beneficiary = policy.Beneficiaries
                .FirstOrDefault(b => b.Email.Equals(beneficiaryEmail, StringComparison.OrdinalIgnoreCase));

            if (beneficiary == null)
                throw new Exception("Beneficiary verification failed: email does not match any authorized beneficiary.");

            return new VerifyLifePolicyResponse
            {
                ApplicationId = policy.Id,
                ClientFullName = $"{policy.Client.FirstName} {policy.Client.FatherName} {policy.Client.GrandFatherName}",
                ClientEmail = policy.Client.User.Email,
                Status = policy.Status,
                Message = $"Policy verified. Beneficiary '{beneficiary.Name}' may now report a life claim."
            };
        }

        public async Task<List<ClaimResponse>> GetMyClaimsAsync(Guid userId)
        {
            // Get client
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null)
                throw new Exception("Client not found.");

            // Get all claims for this client
            var claims = await _context.Claims
                .Where(c => c.ClientId == client.Id)
                .ToListAsync();

            var claimResponses = claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                MotorInsuranceApplicationId = c.MotorInsuranceApplicationId,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId,
                IncidentDate = c.IncidentDate,
                IncidentTime = c.IncidentTime,
                Location = c.Location,
                IncidentType = c.IncidentType,
                Description = c.Description,
                EvidenceImageUrls = !string.IsNullOrEmpty(c.EvidenceImagesJson)
                    ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(c.EvidenceImagesJson)
                    : new List<string>(),
                TrafficPoliceReportPdfUrl = c.TrafficPoliceReportPdfUrl,
                Status = c.Status,
                CreatedAt = c.CreatedAt
            }).ToList();

            return claimResponses;
        }

        public async Task<ClientClaimsByStatusResponse> GetMyClaimsByStatusAsync(Guid userId)
        {
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (client == null)
                throw new Exception("Client not found.");

            var claims = await _context.Claims
                .Where(c => c.ClientId == client.Id)
                .ToListAsync();

            var mappedClaims = claims.Select(c => new ClaimResponse
            {
                ClaimId = c.Id,
                ClientId = c.ClientId,
                MotorInsuranceApplicationId = c.MotorInsuranceApplicationId,
                LifeInsuranceApplicationId = c.LifeInsuranceApplicationId,
                IncidentDate = c.IncidentDate,
                IncidentTime = c.IncidentTime,
                Location = c.Location,
                IncidentType = c.IncidentType,
                Description = c.Description,
                EvidenceImageUrls = !string.IsNullOrEmpty(c.EvidenceImagesJson)
                    ? JsonSerializer.Deserialize<List<string>>(c.EvidenceImagesJson)
                    : new List<string>(),
                TrafficPoliceReportPdfUrl = c.TrafficPoliceReportPdfUrl,
                Status = c.Status,
                CreatedAt = c.CreatedAt
            }).ToList();

            return new ClientClaimsByStatusResponse
            {
                PendingClaims = mappedClaims
            .Where(c =>
                c.Status == ClaimStatus.Pending ||
                c.Status == ClaimStatus.ReviewedByOperator ||
                c.Status == ClaimStatus.ApprovedByManager)
            .ToList(),

                PaidClaims = mappedClaims
            .Where(c => c.Status == ClaimStatus.Paid)
            .ToList(),

                RejectedClaims = mappedClaims
            .Where(c => c.Status == ClaimStatus.Rejected)
            .ToList()
            };
        }

        public async Task<List<ClaimPayoutHistoryResponse>> GetClientPayoutHistoryAsync(Guid clientUserId)
        {
            // Get the client's ID
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId);
            if (client == null) return new List<ClaimPayoutHistoryResponse>();

            var payouts = await _context.Claims
                .Where(c => c.ClientId == client.Id && c.PayoutStatus == ClaimPayoutStatus.Paid)
                .OrderByDescending(c => c.PaidAt)
                .ToListAsync();

            return payouts.Select(c => new ClaimPayoutHistoryResponse
            {
                ClaimId = c.Id,
                ClientFullName = $"{c.Client.FirstName} {c.Client.FatherName} {c.Client.GrandFatherName}",
                ClientPhoneNumber = c.Client.PhoneNumber,
                ApprovedAmount = c.ApprovedAmount ?? 0,
                PaidAt = c.PaidAt!.Value,
                PayoutReference = c.PayoutReference ?? "",
                ClaimStatus = c.Status.ToString(),
                PayoutStatus = c.PayoutStatus.ToString(),
                ClaimDescription = c.Description,
                IncidentDate = c.IncidentDate,
                ClaimType = c.IncidentType?.ToString()
            }).ToList();
        }

    }
}
