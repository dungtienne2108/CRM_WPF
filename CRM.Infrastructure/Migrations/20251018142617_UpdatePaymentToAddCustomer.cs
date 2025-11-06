using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePaymentToAddCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "payment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_payment_CustomerId",
                table: "payment",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_payment_customer_CustomerId",
                table: "payment",
                column: "CustomerId",
                principalTable: "customer",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payment_customer_CustomerId",
                table: "payment");

            migrationBuilder.DropIndex(
                name: "IX_payment_CustomerId",
                table: "payment");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "payment");
        }
    }
}
