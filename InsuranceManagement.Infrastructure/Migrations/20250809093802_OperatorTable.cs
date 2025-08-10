using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OperatorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operator_Users_UserId",
                table: "Operator");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Operators",
                table: "Operators",
                column: "Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_OperatorOperatorCategories_OperatorsId",
                table: "OperatorOperatorCategories",
                column: "OperatorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operators_Users_UserId",
                table: "Operators",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operators_Users_UserId",
                table: "Operators");

            migrationBuilder.DropTable(
                name: "OperatorOperatorCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Operators",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Operator_Users_UserId",
                table: "Operator",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
