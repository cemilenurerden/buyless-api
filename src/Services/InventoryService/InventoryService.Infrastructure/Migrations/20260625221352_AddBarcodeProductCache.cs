using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodeProductCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BarcodeProductCaches",
                columns: table => new
                {
                    Barcode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BrandName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CategoryHint = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LastFetchedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeProductCaches", x => x.Barcode);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodeProductCaches");
        }
    }
}
