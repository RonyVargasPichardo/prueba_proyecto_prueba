using System.ComponentModel.DataAnnotations;

namespace Premialo.Server.Models.Enums
{
    public enum EstatusEnum
    {
        [Display(Name = "Activo")]
        Activo = 1,
        [Display(Name = "Inactivo")]
        Inactivo = 0
    }
}
