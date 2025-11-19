using Premialo.Server.Models;

namespace Premialo.Server.DTOs.Sorteos
{
    // 🔹 Para listar todos los sorteos (GET /api/sorteos)
    public class SorteoTodosNameDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaSorteo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int Premios { get; set; }
        public int Participantes { get; set; }
        public int Resultados { get; set; }
    }

    // 🔹 Para leer un sorteo individual (GET /api/sorteos/{id})
    //    Incluye las colecciones completas (detalles)
    public class SorteoSingleReadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public DateOnly FechaSorteo { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public ICollection<Premio> Premios { get; set; } = new List<Premio>();
        public ICollection<Participante> Participantes { get; set; } = new List<Participante>();
        public ICollection<SorteoResultado> Resultados { get; set; } = new List<SorteoResultado>();
    }

}
