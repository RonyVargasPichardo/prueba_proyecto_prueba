using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Premialo.Server.Migrations
{
    /// <inheritdoc />
    public partial class Agregandobooleano : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "participante",
                table: "Asistencias",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "participante",
                table: "Asistencias");
        }
    }
}
