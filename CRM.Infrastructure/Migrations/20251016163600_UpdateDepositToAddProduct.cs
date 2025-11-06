using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDepositToAddProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "deposit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_deposit_ProductId",
                table: "deposit",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_deposit_product_ProductId",
                table: "deposit",
                column: "ProductId",
                principalTable: "product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_deposit_product_ProductId",
                table: "deposit");

            migrationBuilder.DropIndex(
                name: "IX_deposit_ProductId",
                table: "deposit");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "deposit");
        }
    }
}
