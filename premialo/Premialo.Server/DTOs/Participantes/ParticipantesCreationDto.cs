namespace Premialo.Server.DTOs.Participantes
{
    public class ParticipantesCreationDto
    {
       
            public string DocumentoIdentidad { get; set; } = string.Empty;
            public string Nombres { get; set; } = string.Empty;
            public string Apellidos { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Telefono { get; set; }
            public string Cargo { get; set; } = string.Empty;
            public string UnidadOrganizativa { get; set; } = string.Empty;
          
        

    }
}
