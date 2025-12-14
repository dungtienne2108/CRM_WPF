using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletableToDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCreatedContract",
                table: "deposit",
                newName: "is_created_contract");

            migrationBuilder.AlterColumn<bool>(
                name: "is_created_contract",
                table: "deposit",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "deposit",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "deposit");

            migrationBuilder.RenameColumn(
                name: "is_created_contract",
                table: "deposit",
                newName: "IsCreatedContract");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCreatedContract",
                table: "deposit",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }
    }
}
