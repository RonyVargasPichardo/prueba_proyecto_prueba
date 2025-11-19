using System.ComponentModel.DataAnnotations;

namespace Premialo.server.DTOs.Auth
{
    public class CredentialsDTO
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Contrasena { get; set; }
    }
}
