using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operators_Users_UserId",
                table: "Operators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Operators",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "InvitationToken",
                table: "Operators");

            migrationBuilder.DropColumn(
                name: "InvitationTokenExpiresAt",
                table: "Operators");

            migrationBuilder.RenameTable(
                name: "Operators",
                newName: "Operator");

            migrationBuilder.RenameIndex(
                name: "IX_Operators_UserId",
                table: "Operator",
                newName: "IX_Operator_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Operator",
                table: "Operator",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OperatorCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatorCategories", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Operator_Users_UserId",
                table: "Operator",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operator_Users_UserId",
                table: "Operator");

            migrationBuilder.DropTable(
                name: "OperatorCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Operator",
                table: "Operator");

            migrationBuilder.RenameTable(
                name: "Operator",
                newName: "Operators");

            migrationBuilder.RenameIndex(
                name: "IX_Operator_UserId",
                table: "Operators",
                newName: "IX_Operators_UserId");

            migrationBuilder.AddColumn<string>(
                name: "InvitationToken",
                table: "Operators",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InvitationTokenExpiresAt",
                table: "Operators",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Operators",
                table: "Operators",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Operators_Users_UserId",
                table: "Operators",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
