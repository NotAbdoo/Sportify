using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sportify.Migrations
{
    /// <inheritdoc />
    public partial class AddGymClassesAndOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutUs",
                table: "GymAds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdminCreated",
                table: "GymAds",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "GymAds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkingHours",
                table: "GymAds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GymClasses",
                columns: table => new
                {
                    GymClassId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Price = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageURLs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GymAdId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymClasses", x => x.GymClassId);
                    table.ForeignKey(
                        name: "FK_GymClasses_GymAds_GymAdId",
                        column: x => x.GymAdId,
                        principalTable: "GymAds",
                        principalColumn: "GymAdId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GymOffers",
                columns: table => new
                {
                    GymOfferId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DiscountText = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GymAdId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymOffers", x => x.GymOfferId);
                    table.ForeignKey(
                        name: "FK_GymOffers_GymAds_GymAdId",
                        column: x => x.GymAdId,
                        principalTable: "GymAds",
                        principalColumn: "GymAdId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GymClasses_GymAdId",
                table: "GymClasses",
                column: "GymAdId");

            migrationBuilder.CreateIndex(
                name: "IX_GymOffers_GymAdId",
                table: "GymOffers",
                column: "GymAdId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymClasses");

            migrationBuilder.DropTable(
                name: "GymOffers");

            migrationBuilder.DropColumn(
                name: "AboutUs",
                table: "GymAds");

            migrationBuilder.DropColumn(
                name: "IsAdminCreated",
                table: "GymAds");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "GymAds");

            migrationBuilder.DropColumn(
                name: "WorkingHours",
                table: "GymAds");
        }
    }
}
