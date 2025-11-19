using System.ComponentModel.DataAnnotations;

namespace Premialo.Server.Models.Enums
{
    public enum EstatusPremioEnum
    {
        [Display(Name = "Pendiente")]
        Pendiente = 1,
        [Display(Name = "Sorteado")]
        Sorteado = 2,
        [Display(Name = "Entregado")]
        Entregado = 3,
    }
}
