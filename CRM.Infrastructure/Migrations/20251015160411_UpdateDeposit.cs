using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "deposit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepositName",
                table: "deposit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "deposit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "deposit",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_deposit_ContactId",
                table: "deposit",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_deposit_EmployeeId",
                table: "deposit",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_deposit_contact_ContactId",
                table: "deposit",
                column: "ContactId",
                principalTable: "contact",
                principalColumn: "contact_id");

            migrationBuilder.AddForeignKey(
                name: "FK_deposit_employee_EmployeeId",
                table: "deposit",
                column: "EmployeeId",
                principalTable: "employee",
                principalColumn: "employee_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deposit_contact_ContactId",
                table: "deposit");

            migrationBuilder.DropForeignKey(
                name: "FK_deposit_employee_EmployeeId",
                table: "deposit");

            migrationBuilder.DropIndex(
                name: "IX_deposit_ContactId",
                table: "deposit");

            migrationBuilder.DropIndex(
                name: "IX_deposit_EmployeeId",
                table: "deposit");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "deposit");

            migrationBuilder.DropColumn(
                name: "DepositName",
                table: "deposit");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "deposit");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "deposit");
        }
    }
}
