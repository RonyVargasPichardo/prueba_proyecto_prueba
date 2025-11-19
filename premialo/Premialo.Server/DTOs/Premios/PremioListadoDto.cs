namespace Premialo.Server.DTOs.Premios
{
    public class PremioListadoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int Orden { get; set; }
        public string SorteoNombre { get; set; } = string.Empty;
        public string FechaResultado { get; set; } = string.Empty;

        public string GanadorDocumentoIdentidad { get; set; } = string.Empty;
        public string GanadorNombres { get; set; } = string.Empty;
        public string GanadorApellidos { get; set; } = string.Empty;
        public string GanadorCargo { get; set; } = string.Empty;
        public string GanadorUnidadOrganizativa { get; set; } = string.Empty;

        public string UsuarioCrea { get; set; } = string.Empty;
        public string UsuarioSortea { get; set; } = string.Empty;
        public int Estatus { get; set; }       
        public string EstatusNombre { get; set; }  
    }

}
