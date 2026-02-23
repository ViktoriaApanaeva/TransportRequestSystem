using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTripStartEndFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassengerCount",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "StatusHistory",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "TripEnd",
                table: "Applications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TripStart",
                table: "Applications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TripEnd",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "TripStart",
                table: "Applications");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "StatusHistory",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassengerCount",
                table: "Applications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
