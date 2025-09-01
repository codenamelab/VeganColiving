using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Coliving.BlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedFields1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateActivatedUtc",
                table: "Flats",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Flats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxFloor",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxMonthlyRentalPrice",
                table: "AspNetUsers",
                type: "decimal(18,0)",
                precision: 18,
                scale: 0,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinFloor",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinMonthlyRentalPrice",
                table: "AspNetUsers",
                type: "decimal(18,0)",
                precision: 18,
                scale: 0,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoticePeriodDays",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateActivatedUtc",
                table: "Flats");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Flats");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaxFloor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MaxMonthlyRentalPrice",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MinFloor",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MinMonthlyRentalPrice",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NoticePeriodDays",
                table: "AspNetUsers");
        }
    }
}
