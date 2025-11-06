using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoiceToAddInstallmentSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_invoice_payment_option",
                table: "invoice");

            //migrationBuilder.DropIndex(
            //    name: "IX_invoice_payment_option_id",
            //    table: "invoice");

            migrationBuilder.DropColumn(
                name: "payment_option_id",
                table: "invoice");

            migrationBuilder.DropColumn(
                name: "status",
                table: "invoice");

            migrationBuilder.AddColumn<int>(
                name: "InstallmentScheduleId",
                table: "invoice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "invoice",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_invoice_InstallmentScheduleId",
                table: "invoice",
                column: "InstallmentScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_invoice_installment_schedule_InstallmentScheduleId",
                table: "invoice",
                column: "InstallmentScheduleId",
                principalTable: "installment_schedule",
                principalColumn: "installment_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoice_installment_schedule_InstallmentScheduleId",
                table: "invoice");

            migrationBuilder.DropIndex(
                name: "IX_invoice_InstallmentScheduleId",
                table: "invoice");

            migrationBuilder.DropColumn(
                name: "InstallmentScheduleId",
                table: "invoice");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "invoice");

            migrationBuilder.AddColumn<int>(
                name: "payment_option_id",
                table: "invoice",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "invoice",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "Pending");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_payment_option_id",
                table: "invoice",
                column: "payment_option_id");

            migrationBuilder.AddForeignKey(
                name: "fk_invoice_payment_option",
                table: "invoice",
                column: "payment_option_id",
                principalTable: "payment_option",
                principalColumn: "payment_option_id");
        }
    }
}
