using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coliving.BlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class ExternalUrl1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VeganColiving_FlatEngagement_AspNetUsers_UserId",
                table: "VeganColiving_FlatEngagement");

            migrationBuilder.DropForeignKey(
                name: "FK_VeganColiving_FlatEngagement_VeganColiving_Flat_FlatId",
                table: "VeganColiving_FlatEngagement");

            migrationBuilder.DropForeignKey(
                name: "FK_VeganColiving_Image_VeganColiving_Flat_FlatId",
                table: "VeganColiving_Image");

            migrationBuilder.DropForeignKey(
                name: "FK_VeganColiving_Room_VeganColiving_Flat_FlatId",
                table: "VeganColiving_Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VeganColiving_Room",
                table: "VeganColiving_Room");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VeganColiving_Image",
                table: "VeganColiving_Image");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VeganColiving_FlatEngagement",
                table: "VeganColiving_FlatEngagement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VeganColiving_Flat",
                table: "VeganColiving_Flat");

            migrationBuilder.RenameTable(
                name: "VeganColiving_Room",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "VeganColiving_Image",
                newName: "Images");

            migrationBuilder.RenameTable(
                name: "VeganColiving_FlatEngagement",
                newName: "FlatEngagements");

            migrationBuilder.RenameTable(
                name: "VeganColiving_Flat",
                newName: "Flats");

            migrationBuilder.RenameIndex(
                name: "IX_VeganColiving_Room_FlatId",
                table: "Rooms",
                newName: "IX_Rooms_FlatId");

            migrationBuilder.RenameIndex(
                name: "IX_VeganColiving_Image_FlatId",
                table: "Images",
                newName: "IX_Images_FlatId");

            migrationBuilder.RenameIndex(
                name: "IX_VeganColiving_FlatEngagement_FlatId",
                table: "FlatEngagements",
                newName: "IX_FlatEngagements_FlatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlatEngagements",
                table: "FlatEngagements",
                columns: new[] { "UserId", "FlatId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Flats",
                table: "Flats",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FlatExternalUrls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlatId = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlatExternalUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlatExternalUrls_Flats_FlatId",
                        column: x => x.FlatId,
                        principalTable: "Flats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlatExternalUrls_FlatId",
                table: "FlatExternalUrls",
                column: "FlatId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlatEngagements_AspNetUsers_UserId",
                table: "FlatEngagements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlatEngagements_Flats_FlatId",
                table: "FlatEngagements",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlatEngagements_AspNetUsers_UserId",
                table: "FlatEngagements");

            migrationBuilder.DropForeignKey(
                name: "FK_FlatEngagements_Flats_FlatId",
                table: "FlatEngagements");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Flats_FlatId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Flats_FlatId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "FlatExternalUrls");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Flats",
                table: "Flats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlatEngagements",
                table: "FlatEngagements");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "VeganColiving_Room");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "VeganColiving_Image");

            migrationBuilder.RenameTable(
                name: "Flats",
                newName: "VeganColiving_Flat");

            migrationBuilder.RenameTable(
                name: "FlatEngagements",
                newName: "VeganColiving_FlatEngagement");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_FlatId",
                table: "VeganColiving_Room",
                newName: "IX_VeganColiving_Room_FlatId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_FlatId",
                table: "VeganColiving_Image",
                newName: "IX_VeganColiving_Image_FlatId");

            migrationBuilder.RenameIndex(
                name: "IX_FlatEngagements_FlatId",
                table: "VeganColiving_FlatEngagement",
                newName: "IX_VeganColiving_FlatEngagement_FlatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VeganColiving_Room",
                table: "VeganColiving_Room",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VeganColiving_Image",
                table: "VeganColiving_Image",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VeganColiving_Flat",
                table: "VeganColiving_Flat",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VeganColiving_FlatEngagement",
                table: "VeganColiving_FlatEngagement",
                columns: new[] { "UserId", "FlatId" });

            migrationBuilder.AddForeignKey(
                name: "FK_VeganColiving_FlatEngagement_AspNetUsers_UserId",
                table: "VeganColiving_FlatEngagement",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VeganColiving_FlatEngagement_VeganColiving_Flat_FlatId",
                table: "VeganColiving_FlatEngagement",
                column: "FlatId",
                principalTable: "VeganColiving_Flat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VeganColiving_Image_VeganColiving_Flat_FlatId",
                table: "VeganColiving_Image",
                column: "FlatId",
                principalTable: "VeganColiving_Flat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VeganColiving_Room_VeganColiving_Flat_FlatId",
                table: "VeganColiving_Room",
                column: "FlatId",
                principalTable: "VeganColiving_Flat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
