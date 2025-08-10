using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dimax_front.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TechnicalFileNumber",
                table: "InstallationHistories",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstallationHistories_TechnicalFileNumber",
                table: "InstallationHistories",
                column: "TechnicalFileNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstallationHistories_TechnicalFileNumber",
                table: "InstallationHistories");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicalFileNumber",
                table: "InstallationHistories",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);
        }
    }
}
