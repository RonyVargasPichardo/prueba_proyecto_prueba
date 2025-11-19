using Microsoft.EntityFrameworkCore;
using Premialo.Server.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Premialo.Server.Models
{
    public class Premio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string RutaFoto { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int orden {  get; set; } 

        public EstatusPremioEnum Estatus { get; set; }

        // Relaciones
        [ForeignKey(nameof(Sorteo))]
        public int SorteoId { get; set; }
        public Sorteo Sorteo { get; set; }

        public SorteoResultado Resultado { get; set; }

        [ForeignKey(nameof(UsuarioCreacion))]
        public int UsuarioCreacionId { get; set; }
        public Usuario UsuarioCreacion { get; set; }
    }
}