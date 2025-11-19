namespace Premialo.Server.DTOs.Sorteos
{
    public class SorteoTraerID
    {

        public class PremioDto
        {
            public int Idpremio {get ; set;}
            public string Nombre { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
           
        }

        public class ParticipanteDto
        {

            public int Idparticipante { get; set; }
            public string DocumentoIdentidad { get; set; } = string.Empty;
            public string Nombres { get; set; } = string.Empty;
            public string Apellidos { get; set; } = string.Empty;
            public string Cargo { get; set; } = string.Empty;
            public string UnidadOrganizativa { get; set; } = string.Empty;
            public string Estatus { get; set; }
        }

        public class ResultadoDto
        {  

            public int Idresultado { get; set; }
            public PremioDto Premio { get; set; } = new PremioDto();
            public ParticipanteDto Participante { get; set; } = new ParticipanteDto();
            public DateTime Fecha { get; set; }
        }

        public class SorteoDetalleDto
        {
           
                public int Id { get; set; }
                public string Nombre { get; set; }
                public DateTime FechaSorteo { get; set; }
                public string Descripcion { get; set; }

                public int TotalAsistencias { get; set; }
                public int TotalParticipantes { get; set; }
                public int TotalParticipantesAsistieron { get; set; }
                public int TotalPremios { get; set; }
            public int PremiosSorteados { get; set; }
            public int PremiosPendientes { get; set; }

            public List<ResultadoDto> Resultados { get; set; }
            

        }

    }
}
