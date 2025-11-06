using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationBetweenDepositAndOpportunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_deposit_product",
                table: "deposit");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "deposit",
                newName: "opportunity_id");

            //migrationBuilder.RenameIndex(
            //    name: "IX_deposit_product_id",
            //    table: "deposit",
            //    newName: "IX_deposit_opportunity_id");

            migrationBuilder.CreateIndex(
    name: "IX_deposit_opportunity_id",
    table: "deposit",
    column: "opportunity_id");


            migrationBuilder.AlterColumn<string>(
                name: "product_type_name",
                table: "product_type",
                type: "nvarchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstallmentName",
                table: "installment_schedule",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "installment_schedule",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "contract_type_name",
                table: "contract_type",
                type: "nvarchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "contract_stage_name",
                table: "contract_stage",
                type: "nvarchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "fk_deposit_opportunity",
                table: "deposit",
                column: "opportunity_id",
                principalTable: "opportunity",
                principalColumn: "opportunity_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_deposit_opportunity",
                table: "deposit");

            migrationBuilder.DropColumn(
                name: "InstallmentName",
                table: "installment_schedule");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "installment_schedule");

            migrationBuilder.RenameColumn(
                name: "opportunity_id",
                table: "deposit",
                newName: "product_id");

            migrationBuilder.RenameIndex(
                name: "IX_deposit_opportunity_id",
                table: "deposit",
                newName: "IX_deposit_product_id");

            migrationBuilder.AlterColumn<string>(
                name: "product_type_name",
                table: "product_type",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldUnicode: false,
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "contract_type_name",
                table: "contract_type",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "contract_stage_name",
                table: "contract_stage",
                type: "varchar(255)",
                unicode: false,
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldUnicode: false,
                oldMaxLength: 255);

            migrationBuilder.AddForeignKey(
                name: "fk_deposit_product",
                table: "deposit",
                column: "product_id",
                principalTable: "product",
                principalColumn: "product_id");
        }
    }
}
