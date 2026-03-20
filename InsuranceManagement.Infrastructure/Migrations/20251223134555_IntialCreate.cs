using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OperatorCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    FullInsurancePercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    ThirdPartyPercentage = table.Column<decimal>(type: "numeric", nullable: true),
                    HalfLifePrice = table.Column<decimal>(type: "numeric", nullable: true),
                    FullLifePrice = table.Column<decimal>(type: "numeric", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatorCategories_OperatorCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OperatorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OtpCode = table.Column<string>(type: "text", nullable: true),
                    OtpExpiry = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsOtpVerified = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFatherName = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    NationalIdOrPassport = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    SubCity = table.Column<string>(type: "text", nullable: false),
                    LogoImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PassportOrNationalIdImageUrl = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Finances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFatherName = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    NationalIdOrPassport = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    SubCity = table.Column<string>(type: "text", nullable: false),
                    LogoImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Finances_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Managers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFatherName = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    NationalIdOrPassport = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    SubCity = table.Column<string>(type: "text", nullable: false),
                    LogoImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Managers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Managers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Operators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApprovedByOperatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    FatherName = table.Column<string>(type: "text", nullable: false),
                    GrandFatherName = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    NationalIdOrPassport = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    SubCity = table.Column<string>(type: "text", nullable: false),
                    LogoImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Operators_Operators_ApprovedByOperatorId",
                        column: x => x.ApprovedByOperatorId,
                        principalTable: "Operators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Operators_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LifeInsuranceApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Age = table.Column<float>(type: "real", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    LifePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    LifeInsuranceType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    PaymentReference = table.Column<string>(type: "text", nullable: true),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeInsuranceApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LifeInsuranceApplications_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifeInsuranceApplications_OperatorCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "OperatorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LifeInsuranceApplications_OperatorCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "OperatorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MotorInsuranceApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    PlateNumber = table.Column<string>(type: "text", nullable: false),
                    YearOfManufacture = table.Column<int>(type: "integer", nullable: false),
                    EngineNumber = table.Column<string>(type: "text", nullable: false),
                    ChassisNumber = table.Column<string>(type: "text", nullable: false),
                    MarketPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: true),
                    CalculatedPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    InsuranceType = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    PaymentReference = table.Column<string>(type: "text", nullable: true),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    CarImagePath = table.Column<string>(type: "text", nullable: false),
                    CarLibreImagePath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotorInsuranceApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MotorInsuranceApplications_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MotorInsuranceApplications_OperatorCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "OperatorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MotorInsuranceApplications_OperatorCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "OperatorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperatorOperatorCategories",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperatorsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorOperatorCategories", x => new { x.CategoriesId, x.OperatorsId });
                    table.ForeignKey(
                        name: "FK_OperatorOperatorCategories_OperatorCategories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "OperatorCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OperatorOperatorCategories_Operators_OperatorsId",
                        column: x => x.OperatorsId,
                        principalTable: "Operators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    MotorInsuranceApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    LifeInsuranceApplicationId = table.Column<Guid>(type: "uuid", nullable: true),
                    IncidentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IncidentTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    IncidentType = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    EvidenceImagesJson = table.Column<string>(type: "text", nullable: true),
                    TrafficPoliceReportPdfUrl = table.Column<string>(type: "text", nullable: true),
                    ClaimReason = table.Column<string>(type: "text", nullable: true),
                    DeathCertificatePdf = table.Column<string>(type: "text", nullable: true),
                    MedicalReportPdf = table.Column<string>(type: "text", nullable: true),
                    HospitalDischargeSummaryPdf = table.Column<string>(type: "text", nullable: true),
                    HospitalName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    ApprovedByOperatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovedByManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedByManagerAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Claims_LifeInsuranceApplications_LifeInsuranceApplicationId",
                        column: x => x.LifeInsuranceApplicationId,
                        principalTable: "LifeInsuranceApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Claims_MotorInsuranceApplications_MotorInsuranceApplication~",
                        column: x => x.MotorInsuranceApplicationId,
                        principalTable: "MotorInsuranceApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_ClientId",
                table: "Claims",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_LifeInsuranceApplicationId",
                table: "Claims",
                column: "LifeInsuranceApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_MotorInsuranceApplicationId",
                table: "Claims",
                column: "MotorInsuranceApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Finances_UserId",
                table: "Finances",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LifeInsuranceApplications_CategoryId",
                table: "LifeInsuranceApplications",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeInsuranceApplications_ClientId",
                table: "LifeInsuranceApplications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_LifeInsuranceApplications_SubCategoryId",
                table: "LifeInsuranceApplications",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_UserId",
                table: "Managers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotorInsuranceApplications_CategoryId",
                table: "MotorInsuranceApplications",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MotorInsuranceApplications_ClientId",
                table: "MotorInsuranceApplications",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MotorInsuranceApplications_SubCategoryId",
                table: "MotorInsuranceApplications",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatorCategories_ParentId",
                table: "OperatorCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_OperatorOperatorCategories_OperatorsId",
                table: "OperatorOperatorCategories",
                column: "OperatorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Operators_ApprovedByOperatorId",
                table: "Operators",
                column: "ApprovedByOperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Operators_UserId",
                table: "Operators",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Finances");

            migrationBuilder.DropTable(
                name: "Managers");

            migrationBuilder.DropTable(
                name: "OperatorOperatorCategories");

            migrationBuilder.DropTable(
                name: "LifeInsuranceApplications");

            migrationBuilder.DropTable(
                name: "MotorInsuranceApplications");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "OperatorCategories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
