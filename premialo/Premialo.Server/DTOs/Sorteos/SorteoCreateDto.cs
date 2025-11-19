namespace Premialo.Server.DTOs.Sorteos
{
    public class SorteoCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaSorteo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }


  
}
