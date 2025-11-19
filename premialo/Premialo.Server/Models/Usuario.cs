using Premialo.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Premialo.Server.Models
{
    /// <summary>
    /// Usuario del sistema
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Nombres { get; set; }
        [StringLength(100)]
        public string Apellidos { get; set; }
        [StringLength(200)]
        public string UserName { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(1000)]
        public string PasswordHash { get; set; }
        public RolUsuario Rol { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaUltimoAcceso { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public EstatusEnum Estatus { get; set; } = EstatusEnum.Activo;
        public DateTime? FechaPassword { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public int IntentosLogin { get; set; } = 0;
        public DateTime? BloqueadoHasta { get; set; }
        public bool MustChangePassword { get; set; } = false;

        // Relaciones
        public ICollection<Sorteo> SorteosCreados { get; set; }
        public ICollection<Sorteo> SorteosEditados { get; set; }

        public ICollection<Premio> PremiosCreados { get; set; }
        public ICollection<Participante> ParticipantesCreados { get; set; }
        public ICollection<SorteoResultado> ResultadosCreados { get; set; }
        public ICollection<Asistencia> Asistencias { get; set; }

    }

    public enum RolUsuario
    {
        [Display(Name = "Administrador")]
        Administrador = 1,
        [Display(Name = "Coordinador")]
        Coordinador = 2,
        [Display(Name = "Sorteador")]
        Sorteador = 3,
        [Display(Name = "Registro de Asistencia")]
        Registro_asistencia = 4
    }
}
