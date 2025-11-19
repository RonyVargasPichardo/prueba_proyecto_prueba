namespace Premialo.Server.DTOs.Asistencia
{
    public class ConsultaCedulaDTO
    {
        public string Cedula { get; set; } 
        
        public string Nombres { get; set; }
        public string Apellidos { get; set; }

    }

    public class AsistenciaCreateDto
    {
        public string Cedula { get; set; } = string.Empty;
    }


    public class AsistenciaListadoDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
    }

    public class AsistenciaDashboardDto
    {
        public int TotalAsistencias { get; set; }
        public int TotalParticipantes { get; set; }
        public int TotalNoParticipantes { get; set; }
        public int TotalAsistenciasParticipantes { get; set; }
        public List<AsistenciaListadoDto> Asistencias { get; set; } = new();
    }

}
