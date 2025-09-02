using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coliving.BlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class FlatToHome1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlatExternalUrls_Flats_FlatId",
                table: "FlatExternalUrls");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Flats_FlatId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Flats_FlatId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "FlatEngagements");

            migrationBuilder.DropTable(
                name: "Flats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlatExternalUrls",
                table: "FlatExternalUrls");

            migrationBuilder.RenameTable(
                name: "FlatExternalUrls",
                newName: "ExternalUrls");

            migrationBuilder.RenameColumn(
                name: "FlatId",
                table: "Images",
                newName: "HomeId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_FlatId",
                table: "Images",
                newName: "IX_Images_HomeId");

            migrationBuilder.RenameColumn(
                name: "FlatId",
                table: "ExternalUrls",
                newName: "HomeId");

            migrationBuilder.RenameIndex(
                name: "IX_FlatExternalUrls_FlatId",
                table: "ExternalUrls",
                newName: "IX_ExternalUrls_HomeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExternalUrls",
                table: "ExternalUrls",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Homes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PricePerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DateListed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateActivatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Homes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HomeEngagements",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FlatId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeEngagements", x => new { x.UserId, x.FlatId });
                    table.ForeignKey(
                        name: "FK_HomeEngagements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HomeEngagements_Homes_FlatId",
                        column: x => x.FlatId,
                        principalTable: "Homes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeEngagements_FlatId",
                table: "HomeEngagements",
                column: "FlatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExternalUrls_Homes_HomeId",
                table: "ExternalUrls",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Homes_HomeId",
                table: "Images",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Homes_FlatId",
                table: "Rooms",
                column: "FlatId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExternalUrls_Homes_HomeId",
                table: "ExternalUrls");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Homes_HomeId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Homes_FlatId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "HomeEngagements");

            migrationBuilder.DropTable(
                name: "Homes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExternalUrls",
                table: "ExternalUrls");

            migrationBuilder.RenameTable(
                name: "ExternalUrls",
                newName: "FlatExternalUrls");

            migrationBuilder.RenameColumn(
                name: "HomeId",
                table: "Images",
                newName: "FlatId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_HomeId",
                table: "Images",
                newName: "IX_Images_FlatId");

            migrationBuilder.RenameColumn(
                name: "HomeId",
                table: "FlatExternalUrls",
                newName: "FlatId");

            migrationBuilder.RenameIndex(
                name: "IX_ExternalUrls_HomeId",
                table: "FlatExternalUrls",
                newName: "IX_FlatExternalUrls_FlatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlatExternalUrls",
                table: "FlatExternalUrls",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Flats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateActivatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateListed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImageBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PricePerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlatEngagements",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FlatId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlatEngagements", x => new { x.UserId, x.FlatId });
                    table.ForeignKey(
                        name: "FK_FlatEngagements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlatEngagements_Flats_FlatId",
                        column: x => x.FlatId,
                        principalTable: "Flats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlatEngagements_FlatId",
                table: "FlatEngagements",
                column: "FlatId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlatExternalUrls_Flats_FlatId",
                table: "FlatExternalUrls",
                column: "FlatId",
                principalTable: "Flats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Flats_FlatId",
                table: "Images",
                column: "FlatId",
                principalTable: "Flats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Flats_FlatId",
                table: "Rooms",
                column: "FlatId",
                principalTable: "Flats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
