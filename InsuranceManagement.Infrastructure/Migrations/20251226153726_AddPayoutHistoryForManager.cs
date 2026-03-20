using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayoutHistoryForManager : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FinanceId",
                table: "Claims",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_FinanceId",
                table: "Claims",
                column: "FinanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Finances_FinanceId",
                table: "Claims",
                column: "FinanceId",
                principalTable: "Finances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Finances_FinanceId",
                table: "Claims");

            migrationBuilder.DropIndex(
                name: "IX_Claims_FinanceId",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "FinanceId",
                table: "Claims");
        }
    }
}
