namespace Premialo.Server.DTOs.Sorteos
{
    public class SorteoSingleReadDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaSorteo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
