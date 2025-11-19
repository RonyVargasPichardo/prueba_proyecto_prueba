namespace Premialo.Server.DTOs.Premios
{
    public class PremiosCreateEditionDto
    {

        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Orden { get; set; }
    }
}
