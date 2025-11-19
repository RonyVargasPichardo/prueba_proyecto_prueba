namespace Premialo.Server.DTOs.Premios
{
    public class PremioCargaMasivaDto
    {
      
        public List<PremioItemDto> Premios { get; set; } = new();
    }

    public class PremioItemDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Orden { get; set; }
    }

}
