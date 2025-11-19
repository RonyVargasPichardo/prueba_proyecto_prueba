using System.ComponentModel.DataAnnotations;

namespace Premialo.Server.Models.Enums
{
    public enum ParticipanteEstatusEnum
    {
        [Display(Name = "Registrado")]
        Registrado = 1,
        [Display(Name = "Concursando")]
        Concursando = 2,
        [Display(Name = "Ganador")]
        Ganador = 3,
        [Display(Name = "Premio Reclamado")]
        Reclamado = 4,
    }
}
