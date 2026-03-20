using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MultiBeneficery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeneficiaryEmail",
                table: "LifeInsuranceApplications");

            migrationBuilder.DropColumn(
                name: "BeneficiaryName",
                table: "LifeInsuranceApplications");

            migrationBuilder.DropColumn(
                name: "BeneficiaryNationalIdPath",
                table: "LifeInsuranceApplications");

            migrationBuilder.DropColumn(
                name: "BeneficiaryPhoneNumber",
                table: "LifeInsuranceApplications");

            migrationBuilder.DropColumn(
                name: "BeneficiaryRelation",
                table: "LifeInsuranceApplications");

            migrationBuilder.CreateTable(
                name: "LifeInsuranceBeneficiary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LifeInsuranceApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Relation = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    NationalIdFilePath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifeInsuranceBeneficiary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LifeInsuranceBeneficiary_LifeInsuranceApplications_LifeInsu~",
                        column: x => x.LifeInsuranceApplicationId,
                        principalTable: "LifeInsuranceApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifeInsuranceBeneficiary_LifeInsuranceApplicationId",
                table: "LifeInsuranceBeneficiary",
                column: "LifeInsuranceApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifeInsuranceBeneficiary");

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryEmail",
                table: "LifeInsuranceApplications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryName",
                table: "LifeInsuranceApplications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryNationalIdPath",
                table: "LifeInsuranceApplications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BeneficiaryPhoneNumber",
                table: "LifeInsuranceApplications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BeneficiaryRelation",
                table: "LifeInsuranceApplications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
