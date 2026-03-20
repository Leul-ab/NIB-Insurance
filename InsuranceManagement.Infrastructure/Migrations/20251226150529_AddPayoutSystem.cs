using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayoutSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Claims",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayoutFailureReason",
                table: "Claims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PayoutReference",
                table: "Claims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PayoutStatus",
                table: "Claims",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "PayoutFailureReason",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "PayoutReference",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "PayoutStatus",
                table: "Claims");
        }
    }
}
