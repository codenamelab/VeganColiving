using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coliving.BlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class RoomsAndPicture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VeganColiving_Flat_AspNetUsers_ApplicationUserId",
                table: "VeganColiving_Flat");

            migrationBuilder.DropIndex(
                name: "IX_VeganColiving_Flat_ApplicationUserId",
                table: "VeganColiving_Flat");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "VeganColiving_Flat");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageBytes",
                table: "VeganColiving_Flat",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "VeganColiving_Flat",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VeganColiving_Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlatId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PricePerMonth = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AvailableFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvailableTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    SizeSqm = table.Column<double>(type: "float", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeganColiving_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeganColiving_Room_VeganColiving_Flat_FlatId",
                        column: x => x.FlatId,
                        principalTable: "VeganColiving_Flat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VeganColiving_Room_FlatId",
                table: "VeganColiving_Room",
                column: "FlatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VeganColiving_Room");

            migrationBuilder.DropColumn(
                name: "ImageBytes",
                table: "VeganColiving_Flat");

            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "VeganColiving_Flat");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "VeganColiving_Flat",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VeganColiving_Flat_ApplicationUserId",
                table: "VeganColiving_Flat",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_VeganColiving_Flat_AspNetUsers_ApplicationUserId",
                table: "VeganColiving_Flat",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
