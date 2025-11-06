using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContractTableToAddProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ContractValuePercentage",
                table: "installment_schedule",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "contract",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountAfterTax",
                table: "contract",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountBeforeTax",
                table: "contract",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ContractNumber",
                table: "contract",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DepositId",
                table: "contract",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "contract",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Seller",
                table: "contract",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "contract",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_contract_DepositId",
                table: "contract",
                column: "DepositId");

            migrationBuilder.CreateIndex(
                name: "IX_contract_ProductId",
                table: "contract",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_contract_deposit_DepositId",
                table: "contract",
                column: "DepositId",
                principalTable: "deposit",
                principalColumn: "deposit_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contract_product_ProductId",
                table: "contract",
                column: "ProductId",
                principalTable: "product",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contract_deposit_DepositId",
                table: "contract");

            migrationBuilder.DropForeignKey(
                name: "FK_contract_product_ProductId",
                table: "contract");

            migrationBuilder.DropIndex(
                name: "IX_contract_DepositId",
                table: "contract");

            migrationBuilder.DropIndex(
                name: "IX_contract_ProductId",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "ContractValuePercentage",
                table: "installment_schedule");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "AmountAfterTax",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "AmountBeforeTax",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "ContractNumber",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "DepositId",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "Seller",
                table: "contract");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "contract");
        }
    }
}
