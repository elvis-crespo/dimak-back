using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dimax_front.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicules",
                columns: table => new
                {
                    Plate = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicules", x => x.Plate);
                });

            migrationBuilder.CreateTable(
                name: "InstallationHistories",
                columns: table => new
                {
                    HistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: false),
                    TechnicalFileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    TechnicianName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    InstallationCompleted = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PlateId = table.Column<string>(type: "nvarchar(8)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallationHistories", x => x.HistoryId);
                    table.ForeignKey(
                        name: "FK_InstallationHistories_Vehicules_PlateId",
                        column: x => x.PlateId,
                        principalTable: "Vehicules",
                        principalColumn: "Plate",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstallationHistories_InvoiceNumber",
                table: "InstallationHistories",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstallationHistories_PlateId",
                table: "InstallationHistories",
                column: "PlateId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id_Username_Email",
                table: "Users",
                columns: new[] { "Id", "Username", "Email" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstallationHistories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vehicules");
        }
    }
}
