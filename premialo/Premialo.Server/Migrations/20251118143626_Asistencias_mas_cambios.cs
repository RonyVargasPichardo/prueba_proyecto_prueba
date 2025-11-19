using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Premialo.Server.Migrations
{
    /// <inheritdoc />
    public partial class Asistencias_mas_cambios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Asistencias",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Apellidos",
                table: "Asistencias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombres",
                table: "Asistencias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
                create or ALTER PROCEDURE [dbo].[sp_Consulta_Cedula_HHRR]
                	@Documento char(13)= null
                AS
                BEGIN
                
                	SET NOCOUNT ON;
                
                
                	IF (LEN(@Documento))= 13
                SELECT 
                 E.Cedula, E.Nombres, E.Apellidos
                  FROM db_RRHH.dbo.Empleado E
                 where cedula= @Documento

                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apellidos",
                table: "Asistencias");

            migrationBuilder.DropColumn(
                name: "Nombres",
                table: "Asistencias");

            migrationBuilder.AlterColumn<string>(
                name: "Cedula",
                table: "Asistencias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);
        }
    }
}
