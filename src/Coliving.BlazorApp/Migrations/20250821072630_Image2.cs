using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coliving.BlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class Image2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlatId",
                table: "VeganColiving_Image",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VeganColiving_Image_FlatId",
                table: "VeganColiving_Image",
                column: "FlatId");

            migrationBuilder.AddForeignKey(
                name: "FK_VeganColiving_Image_VeganColiving_Flat_FlatId",
                table: "VeganColiving_Image",
                column: "FlatId",
                principalTable: "VeganColiving_Flat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VeganColiving_Image_VeganColiving_Flat_FlatId",
                table: "VeganColiving_Image");

            migrationBuilder.DropIndex(
                name: "IX_VeganColiving_Image_FlatId",
                table: "VeganColiving_Image");

            migrationBuilder.DropColumn(
                name: "FlatId",
                table: "VeganColiving_Image");
        }
    }
}
