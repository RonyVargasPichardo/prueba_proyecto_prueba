using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Premialo.Server.Migrations
{
    /// <inheritdoc />
    public partial class Estatus_premio_add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estatus",
                table: "Premios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estatus",
                table: "Premios");
        }
    }
}
