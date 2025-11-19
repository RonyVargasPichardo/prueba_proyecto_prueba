using Premialo.Server.Models.Enums;

namespace Premialo.Server.DTOs.Sorteos
{
    public class SorteoUpdateDto
    {
        public int? Id { get; set; }
        public string? Nombre { get; set; } = string.Empty;
        public DateTime  FechaSorteo { get; set; }
        public string? Descripcion { get; set; } = string.Empty;

        // Opcional: si no se envía, se mantiene igual
        public SorteoEstatusEnum? Estatus { get; set; }
    }

}
