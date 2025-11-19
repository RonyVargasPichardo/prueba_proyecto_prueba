using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.OpenApi.Extensions;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Participantes;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Models;
using Premialo.Server.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Premialo.server.Controllers
{



    [Route("api/sorteos")]
    [ApiController]
    public class ParticipantesController : ControllerBase
    {
        private readonly ParticipantesService _service;
        public ParticipantesController(ParticipantesService service)
        {
            _service = service;
        }

        [HttpPost()]
        [Route("{sorteoId:int}/participantes/bulk")]
        [SwaggerOperation(
            Summary = "Entrada de la lista de participantes tipo Json",
            Description = "**Descripción:** Aqui se debe subir la lista de participantes de cada sorteo en formato json."
        )]
        public async Task<ActionResult<IEnumerable<ParticipantesCreationDto>>> CrearLote(int sorteoId, [FromBody] List<ParticipantesCreationDto> lista)
        {
            if (lista == null || lista.Count == 0)
                return BadRequest("Debe enviar al menos un participante.");

            var creados = await _service.CrearParticipantesJsonAsync(sorteoId, lista);

            return Ok(new
            {
                sorteoId,
                total = creados,
                mensaje = "Participantes agregados correctamente."
            });
        }

        [HttpGet()]
        [Route("{sorteoId:int}/participantes/{participanteId:int}")]
        [SwaggerOperation(
            Summary = "Obtener participante especifico",
            Description = "**Descripción:** Permite traer de base de datos un participante en especifico usando el id del sorteo y el id del participante."
        )]
        public async Task<IActionResult> ParticipantesIDAsync(int sorteoId, int participanteId)
        {
            var data = await _service.ParticipantesIDAsync(sorteoId, participanteId);

            if (data == null)
                return NotFound();

            return Ok(data);
        }
        [HttpGet()]
        [Route("{sorteoId:int}/participantes")]
        [SwaggerOperation(
            Summary = "Lista de participantes de un sorteo con filtros",
            Description = "**Descripción:** Nos treae la lista de participantes de un sorteo en especifico buscando por id ademas nos permite usar varios filtros para mejor busqueda ."
        )]
        public async Task<IActionResult> BuscarParticipantes(
            int sorteoId,
            [FromQuery] string? nombre,
            [FromQuery] string? apellidos,
            [FromQuery] string? documento,
            [FromQuery] string? cargo,
            [FromQuery] string? unidad)
        {
            var resultado = await _service.ParticipantesFiltros(
                sorteoId, nombre, apellidos, documento, cargo, unidad
            );

            return Ok(resultado);
        }


        [HttpDelete()]
        [Route("{sorteoId:int}/participantes/{participanteId:int}")]
        [SwaggerOperation(
            Summary = "Eliminar participante de un sorteo",
            Description = "**Descripción:** Nos deja eliminar un participante de un sorteo usando el id del sorteo y el id del participante"
        )]
        public async Task<IActionResult> Eliminar(int sorteoId, int participanteId)
        {
            var eliminado = await _service.EliminarParticipantesAsync(sorteoId, participanteId);

            if (!eliminado)
                return NotFound();

            return NoContent(); // 204
        }

        [HttpDelete("{sorteoId:int}/participantes")]
        [SwaggerOperation(
            Summary = "Eliminar todos los participantes de un sorteo",
            Description = "**Descripción:** Nos deja eliminar todos los participantes de un sorteo"
        )]
        public async Task<IActionResult> EliminarTodos(int sorteoId)
        {
            var total = await _service.EliminarTodosParticipantesAsync(sorteoId);

            return Ok(new
            {
                SorteoId = sorteoId,
                Eliminados = total,
                Mensaje = "Todos los participantes fueron eliminados correctamente."
            });
        }


    }
}
