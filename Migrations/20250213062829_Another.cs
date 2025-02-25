using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dimax_front.Migrations
{
    /// <inheritdoc />
    public partial class Another : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InstallationHistories_InvoiceNumber",
                table: "InstallationHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InstallationHistories_InvoiceNumber",
                table: "InstallationHistories",
                column: "InvoiceNumber",
                unique: true,
                filter: "[InvoiceNumber] IS NOT NULL");
        }
    }
}
