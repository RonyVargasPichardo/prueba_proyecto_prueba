using Premialo.Server.Models;
using Premialo.Server.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Premialo.Server.DTOs.Usuarios
{
    public class UsuarioDTO
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
        
        public EstatusEnum Estatus { get; set; }
        public DateTime? FechaPassword { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public int IntentosLogin { get; set; } = 0;
        public DateTime? BloqueadoHasta { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
