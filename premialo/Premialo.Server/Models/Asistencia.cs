using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Premialo.Server.Models
{
    public class Asistencia
    {

        [Key]
        public int Id { get; set; }
        [StringLength(13)]
        public string Cedula { get; set; }
        
        [StringLength(100)]
        public string Nombres { get; set; }

        [StringLength(100)]
        public string Apellidos { get; set; }

        public DateTime? Fecha { get; set; }
        public bool participante {  get; set; }

        [ForeignKey(nameof(Sorteo))]
        public int SorteoId { get; set; }
        public Sorteo Sorteo { get; set; }

        [ForeignKey(nameof(UsuarioCreacion))]
        public int UsuarioCreacionId { get; set; }
        public Usuario UsuarioCreacion { get; set; }
    }
}
