using Microsoft.EntityFrameworkCore;
using Premialo.Server.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Premialo.Server.Models
{
    [Index(nameof(Nombre), IsUnique = true)]
    public class Sorteo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaSorteo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; } 
        public DateTime? FechaModificacion { get; set; }
        public SorteoEstatusEnum Estatus { get; set; } = SorteoEstatusEnum.Activo;

        // Relaciones
        public ICollection<Premio> Premios { get; set; }
        public ICollection<Participante> Participantes { get; set; }
        public ICollection<SorteoResultado> Resultados { get; set; }
        public ICollection <Asistencia> Asistencias { get; set; }

        [ForeignKey(nameof(UsuarioCreacion))]
        public int UsuarioCreacionId { get; set; }
        public Usuario UsuarioCreacion { get; set; }

        [ForeignKey(nameof(UsuarioModificacion))]
        public int? UsuarioModificacionId { get; set; }
        public Usuario? UsuarioModificacion { get; set; }
    }
}
