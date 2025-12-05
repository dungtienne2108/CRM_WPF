using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "contact_type_id",
                table: "contact",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "contact_type",
                columns: table => new
                {
                    contact_type_id = table.Column<int>(type: "int", nullable: false),
                    contact_type_code = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    contact_type_name = table.Column<string>(type: "nvarchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__contact___3A81B3272D8C2C1E", x => x.contact_type_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contact_contact_type_id",
                table: "contact",
                column: "contact_type_id");

            migrationBuilder.CreateIndex(
                name: "UQ__contact___D4C9E3B2E2E2F6C3",
                table: "contact_type",
                column: "contact_type_code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_contact_type",
                table: "contact",
                column: "contact_type_id",
                principalTable: "contact_type",
                principalColumn: "contact_type_id");

            // seed initial contact types
            migrationBuilder.InsertData(
                table: "contact_type",
                columns: new[] { "contact_type_id", "contact_type_code", "contact_type_name" },
                values: new object[,]
                {
                    { 1, "STAFF", "Nhân viên" },
                    { 2, "REPRESENTATIVE", "Người đại diện" },
                    { 3, "DIRECT", "Giám đốc" },
                    { 4, "HEAD", "Trưởng phòng" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contact_type",
                table: "contact");

            migrationBuilder.DropTable(
                name: "contact_type");

            migrationBuilder.DropIndex(
                name: "IX_contact_contact_type_id",
                table: "contact");

            migrationBuilder.DropColumn(
                name: "contact_type_id",
                table: "contact");
        }
    }
}
