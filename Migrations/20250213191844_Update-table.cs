using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dimax_front.Migrations
{
    /// <inheritdoc />
    public partial class Updatetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstallationHistories_InvoiceNumber",
                table: "InstallationHistories");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicianName",
                table: "InstallationHistories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "InstallationHistories",
                type: "nvarchar(17)",
                maxLength: 17,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(17)",
                oldMaxLength: 17);

            migrationBuilder.CreateIndex(
                name: "IX_InstallationHistories_InvoiceNumber",
                table: "InstallationHistories",
                column: "InvoiceNumber",
                unique: true,
                filter: "[InvoiceNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstallationHistories_InvoiceNumber",
                table: "InstallationHistories");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicianName",
                table: "InstallationHistories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceNumber",
                table: "InstallationHistories",
                type: "nvarchar(17)",
                maxLength: 17,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(17)",
                oldMaxLength: 17,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstallationHistories_InvoiceNumber",
                table: "InstallationHistories",
                column: "InvoiceNumber",
                unique: true);
        }
    }
}
