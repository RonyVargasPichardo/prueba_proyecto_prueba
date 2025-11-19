using Premialo.Server.Models.Enums;

namespace Premialo.Server.DTOs.Participantes
{
    public class ParticipantesDto
    {
        public int Id { get; set; }
        public string DocumentoIdentidad { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string UnidadOrganizativa { get; set; } = string.Empty;
        public string FechaRegistro { get; set; } = string.Empty;
        public ParticipanteEstatusEnum Estatus { get; set; } = ParticipanteEstatusEnum.Registrado;
        public string SorteoNombre { get; set; } = string.Empty;
        public string PremioNombre { get; set; } = string.Empty;
        public string UsuarioCrea { get; set; } = string.Empty;
    }

    public class ParticipanteListFiltrosDto
    {
        public int Id { get; set; }
        public string DocumentoIdentidad { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string UnidadOrganizativa { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
        public string FechaRegistro { get; set; } = string.Empty;
    }

}
