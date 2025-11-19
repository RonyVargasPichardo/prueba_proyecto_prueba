using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Premios;
using Premialo.Server.DTOs.Sorteos;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Models;
using Premialo.Server.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Expressions;
using static Premialo.Server.DTOs.Sorteos.SorteoTraerID;

namespace Premialo.server.Controllers
{
    [Route("api/sorteos")]
    [ApiController]
    [Authorize]
    public class SorteoController : ControllerBase
    {
        private readonly SorteoService _service;
        public SorteoController(SorteoService servise)
        {
            _service = servise;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Traer toda la informacion de todos los sorteos",
            Description = "**Descripción:** Permite acceder a toda la infomacion de todos los sorteos"
        )]
        public async Task<ActionResult<IEnumerable<SorteoTodosNameDto>>> GetAll()
        {
            var lista = await _service.ObtenerTodosAsync();
            return Ok(lista);
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Ver sorteos por ID",
            Description = "**Descripción:** Nos trae toda la infomacion de un sorteo accediendo mediante su id."
        )]
        public async Task<ActionResult<IEnumerable<SorteoDetalleDto>>> GetById(int id)
        {
            var sorteo = await _service.ObtenerPorIdAsync(id);
            if (sorteo == null) return NotFound();
            return Ok(sorteo);
        }

        [HttpGet("{id:int}/single")]
        [SwaggerOperation(
            Summary = "Ver sorteo por ID",
            Description = "**Descripción:** Nos trae solo la infomacion básica de un sorteo accediendo mediante su id."
        )]
        public async Task<ActionResult<IEnumerable<SorteoSingleReadDTO>>> GetSingleById(int id)
        {
            var sorteo = await _service.GetSorteoSingle(id);
            if (sorteo == null) return NotFound();
            return Ok(sorteo);
        }


        [HttpPost]
        [SwaggerOperation(
            Summary = "Crear sorteo",
            Description = "**Descripción:** Nos permire crear nuevo sorteos."
        )]
        public async Task<ActionResult<IEnumerable<SorteoCreateDto>>> Crear([FromBody] SorteoCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Datos inválidos.");

            var nuevo = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = nuevo.Id }, nuevo);
        }


        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Edicion de sorteos",
            Description = "**Descripción:** Nos permite editar los sorteos."
        )]
        public async Task<ActionResult<IEnumerable<SorteoUpdateDto>>> Editar(int id, [FromBody] SorteoUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest("El ID del cuerpo no coincide con el parámetro.");

            var actualizado = await _service.EditarAsync(dto);
            return Ok(actualizado);
        }


        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Eliminar sorteos",
            Description = "**Descripción:** Permite eliminar sorteos por un id ."
        )]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }

        [HttpGet]
        [Route("{id:int}/proximo-premio")]
        [SwaggerOperation(
            Summary = "Obtiene el próximo premio a sortear",
            Description = "El próximo premio es el siguiente en el orden, en estatus \"Pendiente\", que no tiene resultados."
        )]
        [ProducesResponseType(typeof(ResultadoResponseDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<ProximoPremioDTO>> GetProximoPremio(int id)
        {
            var result = await _service.GetProximoPremioAsync(id);

            if (result == null) return NotFound("No hay mas premios disponibles para sortear");

            return Ok(result);
        }

        [HttpPost]
        [Route("{id:int}/sortear")]
        [SwaggerOperation(
            Summary = "Obtiene el resultado del sorteo del próximo premio",
            Description = "El próximo premio es el siguiente en el orden, en estatus \"Pendiente\", que no tiene resultados. El ganador debe de estar en estatus \"Concursando\" y no tener resultados"
        )]
        public async Task<ActionResult<ResultadoResponseDTO>> Sortear(int id)
        {
            try
            {
                var result = await _service.Sortear(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No hay premios") || ex.Message.Contains("No hay participantes")) return NotFound(ex.Message);

                throw;
            }
        }
    }
}
