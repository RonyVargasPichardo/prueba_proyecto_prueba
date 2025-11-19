using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Premialo.Server.Migrations
{
    /// <inheritdoc />
    public partial class Creaciondelmodelodeasistencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SorteoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asistencias_Sorteos_SorteoId",
                        column: x => x.SorteoId,
                        principalTable: "Sorteos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Asistencias_Usuarios_UsuarioCreacionId",
                        column: x => x.UsuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_SorteoId",
                table: "Asistencias",
                column: "SorteoId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_UsuarioCreacionId",
                table: "Asistencias",
                column: "UsuarioCreacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Asistencias");
        }
    }
}
