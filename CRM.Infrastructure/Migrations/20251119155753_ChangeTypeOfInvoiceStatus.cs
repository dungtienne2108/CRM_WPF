using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTypeOfInvoiceStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "invoice");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "invoice",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "invoice");

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
