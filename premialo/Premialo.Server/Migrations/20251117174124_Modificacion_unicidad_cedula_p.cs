using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Premialo.Server.Migrations
{
    /// <inheritdoc />
    public partial class Modificacion_unicidad_cedula_p : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participantes_DocumentoIdentidad",
                table: "Participantes");

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_DocumentoIdentidad_SorteoId",
                table: "Participantes",
                columns: new[] { "DocumentoIdentidad", "SorteoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Participantes_DocumentoIdentidad_SorteoId",
                table: "Participantes");

            migrationBuilder.CreateIndex(
                name: "IX_Participantes_DocumentoIdentidad",
                table: "Participantes",
                column: "DocumentoIdentidad",
                unique: true);
        }
    }
}
