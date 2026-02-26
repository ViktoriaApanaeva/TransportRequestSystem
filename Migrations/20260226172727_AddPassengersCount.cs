using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransportRequestSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPassengersCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Passengers",
                table: "Applications",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Passengers",
                table: "Applications");
        }
    }
}
