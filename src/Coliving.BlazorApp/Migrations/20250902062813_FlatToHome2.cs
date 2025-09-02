using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coliving.BlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class FlatToHome2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeEngagements_Homes_FlatId",
                table: "HomeEngagements");

            migrationBuilder.RenameColumn(
                name: "FlatId",
                table: "HomeEngagements",
                newName: "HomeId");

            migrationBuilder.RenameIndex(
                name: "IX_HomeEngagements_FlatId",
                table: "HomeEngagements",
                newName: "IX_HomeEngagements_HomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeEngagements_Homes_HomeId",
                table: "HomeEngagements",
                column: "HomeId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeEngagements_Homes_HomeId",
                table: "HomeEngagements");

            migrationBuilder.RenameColumn(
                name: "HomeId",
                table: "HomeEngagements",
                newName: "FlatId");

            migrationBuilder.RenameIndex(
                name: "IX_HomeEngagements_HomeId",
                table: "HomeEngagements",
                newName: "IX_HomeEngagements_FlatId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeEngagements_Homes_FlatId",
                table: "HomeEngagements",
                column: "FlatId",
                principalTable: "Homes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
