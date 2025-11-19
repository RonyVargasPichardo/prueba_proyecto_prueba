using Swashbuckle.AspNetCore.Annotations;

namespace Premialo.Server.DTOs.Sorteos
{
    [SwaggerSchema("Representa la respuesta de un resultado de sorteo.")]
    public class ResultadoResponseDTO
    {
        public int Id { get; set; }  
        public DateTime Fecha { get; set; }
        public PremioIfoDTO Premio { get; set; }
        public ParticipanteInfoDTO Participante { get; set; }
    }

    [SwaggerSchema("Representa la información básica de un premio.")]
    public class PremioIfoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }

    [SwaggerSchema("Representa la información básica de un participante.")]
    public class ParticipanteInfoDTO
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        [SwaggerSchema("Número de documento de identidad del participante.")]
        public string DocumentoIdentidad { get; set; }
        public string Cargo { get; set; }
        [SwaggerSchema("Unidad a la que pertenece el participante (dirección, departamento, sección, etc...).")]
        public string Unidad { get; set; }
    }

}
