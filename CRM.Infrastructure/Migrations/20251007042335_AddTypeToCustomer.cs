using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerTypeId",
                table: "customer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_CustomerTypeId",
                table: "customer",
                column: "CustomerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_customer_type_CustomerTypeId",
                table: "customer",
                column: "CustomerTypeId",
                principalTable: "customer_type",
                principalColumn: "customer_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_customer_type_CustomerTypeId",
                table: "customer");

            migrationBuilder.DropIndex(
                name: "IX_customer_CustomerTypeId",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "CustomerTypeId",
                table: "customer");
        }
    }
}
