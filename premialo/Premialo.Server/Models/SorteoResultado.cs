using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Premialo.Server.Models
{
    [Index(nameof(SorteoId), nameof(PremioId), nameof(ParticipanteId), IsUnique = true)]
    public class SorteoResultado
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        // Relaciones
        [ForeignKey(nameof(Sorteo))]
        public int SorteoId { get; set; }
        public Sorteo Sorteo { get; set; }
        
        [ForeignKey(nameof(Participante))]
        public int ParticipanteId { get; set; }
        public Participante Participante { get; set; }

        [ForeignKey(nameof(Premio))]
        public int PremioId { get; set; }
        public Premio Premio { get; set; }

        [ForeignKey(nameof(UsuarioCreacion))]
        public int UsuarioCreacionId { get; set; }
        public Usuario UsuarioCreacion { get; set; }

        internal string? FirstOrDefault()
        {
            throw new NotImplementedException();
        }
    }
}