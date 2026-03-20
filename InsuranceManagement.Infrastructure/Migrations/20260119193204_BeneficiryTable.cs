using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BeneficiryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LifeInsuranceBeneficiary_LifeInsuranceApplications_LifeInsu~",
                table: "LifeInsuranceBeneficiary");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LifeInsuranceBeneficiary",
                table: "LifeInsuranceBeneficiary");

            migrationBuilder.RenameTable(
                name: "LifeInsuranceBeneficiary",
                newName: "Beneficiaries");

            migrationBuilder.RenameIndex(
                name: "IX_LifeInsuranceBeneficiary_LifeInsuranceApplicationId",
                table: "Beneficiaries",
                newName: "IX_Beneficiaries_LifeInsuranceApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Beneficiaries",
                table: "Beneficiaries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Beneficiaries_LifeInsuranceApplications_LifeInsuranceApplic~",
                table: "Beneficiaries",
                column: "LifeInsuranceApplicationId",
                principalTable: "LifeInsuranceApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Beneficiaries_LifeInsuranceApplications_LifeInsuranceApplic~",
                table: "Beneficiaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Beneficiaries",
                table: "Beneficiaries");

            migrationBuilder.RenameTable(
                name: "Beneficiaries",
                newName: "LifeInsuranceBeneficiary");

            migrationBuilder.RenameIndex(
                name: "IX_Beneficiaries_LifeInsuranceApplicationId",
                table: "LifeInsuranceBeneficiary",
                newName: "IX_LifeInsuranceBeneficiary_LifeInsuranceApplicationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LifeInsuranceBeneficiary",
                table: "LifeInsuranceBeneficiary",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LifeInsuranceBeneficiary_LifeInsuranceApplications_LifeInsu~",
                table: "LifeInsuranceBeneficiary",
                column: "LifeInsuranceApplicationId",
                principalTable: "LifeInsuranceApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
