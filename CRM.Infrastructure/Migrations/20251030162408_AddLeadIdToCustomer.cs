using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadIdToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeadId",
                table: "customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_LeadId",
                table: "customer",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_lead_LeadId",
                table: "customer",
                column: "LeadId",
                principalTable: "lead",
                principalColumn: "lead_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_lead_LeadId",
                table: "customer");

            migrationBuilder.DropIndex(
                name: "IX_customer_LeadId",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "customer");
        }
    }
}
