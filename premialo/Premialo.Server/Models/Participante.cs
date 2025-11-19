using Microsoft.EntityFrameworkCore;
using Premialo.Server.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Premialo.Server.Models
{
    [Index(nameof(DocumentoIdentidad), nameof(SorteoId), IsUnique = true)]
    public class Participante
    {
        public int Id { get; set; }
        public string DocumentoIdentidad { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string Cargo { get; set; }
        public string UnidadOrganizativa { get; set; } // Departmento, dirección o area
        public DateTime FechaRegistro { get; set; }
        public ParticipanteEstatusEnum Estatus { get; set; } = ParticipanteEstatusEnum.Registrado;

        // Relaciones
        [ForeignKey(nameof(Sorteo))]
        public int SorteoId { get; set; }
        public Sorteo Sorteo { get; set; }

        public SorteoResultado? Resultado { get; set; }

        [ForeignKey(nameof(UsuarioCreacion))]
        public int UsuarioCreacionId { get; set; }
        public Usuario UsuarioCreacion { get; set; }
    }
}