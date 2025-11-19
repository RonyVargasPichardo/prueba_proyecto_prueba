using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using Premialo.server.DTOs.Auth;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Participantes;
using Premialo.Server.DTOs.Premios;
using Premialo.Server.DTOs.Sorteos;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Models;
using Premialo.Server.Services;
using Premialo.Server.Services;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Annotations;
using System.Transactions;

namespace Premialo.server.Controllers
{

    [Route("api/sorteos")]
    [ApiController]
    [Authorize]
    public class PremiosController : ControllerBase
    {
        private readonly PremiosService _service;
        public PremiosController(PremiosService service)
        {
            _service = service;
        }


        [HttpGet("{sorteoId:int}/premios")]
        [SwaggerOperation(
            Summary = "Traer todos los premios por el id ademas te tener un filtro con el nombre"
        )]
        public async Task<ActionResult<IEnumerable<PremioListadoDto>>> GetPremios(
            int sorteoId,
            [FromQuery] string? nombre)
        {
            var lista = await _service.ObtenerPremiosAsync(sorteoId, nombre);
            return Ok(lista);
        }

        [HttpGet("{sorteoId:int}/premios/{id:int}")]
        [SwaggerOperation(
            Summary = "Trae todos los premios por el id Sorteo ademas de filtrar usando el IdPremio "
        )]
        public async Task<ActionResult<IEnumerable<PremioListadoDto>>> ObtenerPremio(int sorteoId, int id)
        {
            var premio = await _service.ObtenerPremioPorIdAsync(sorteoId, id);

            if (premio == null)
                return NotFound();

            return Ok(premio);
        }



        [HttpPost("{sorteoId:int}/premios")]
        [SwaggerOperation(
            Summary = "Creacion de un solo premio",
            Description = "Creacion de un solo premio usando el id sorteo"
        )]
        public async Task<ActionResult<IEnumerable<PremiosCreateEditionDto>>> Crear(int sorteoId, [FromBody] PremiosCreateEditionDto dto)
        {
            var nuevo = await _service.CrearPremioAsync(sorteoId, dto);
            return Created();
        }

        [HttpPost("{sorteoId:int}/premios/json")]
        [SwaggerOperation(
            Summary = "Creacion de varios premios usando Json"

        )]
        public async Task<ActionResult<IEnumerable<PremioCargaMasivaDto>>> CrearMasivo(int sorteoId, [FromBody] PremioCargaMasivaDto dto)
        {
            if (dto.Premios == null || dto.Premios.Count == 0)
                return BadRequest("Debe enviar al menos un premio.");

            var total = await _service.CrearPremiosMasivosAsync(sorteoId, dto);

            return Ok(new
            {
                SorteoId = sorteoId,
                TotalInsertados = total,
                Mensaje = "Premios creados correctamente."
            });
        }


        // PUT: api/sorteos/{sorteoId}/premios/{id}
        [HttpPut("{sorteoId:int}/premios/{id:int}")]
        [SwaggerOperation(
           Summary = "Edicion de los premios ya creados usando id sorteo y id premio "

       )]
        public async Task<ActionResult<IEnumerable<PremiosCreateEditionDto>>> Editar(int sorteoId, int id, [FromBody] PremiosCreateEditionDto dto)
        {
            var actualizado = await _service.EditarPremioAsync(sorteoId, id, dto);

            if (actualizado == null)
                return NotFound();

            return Ok(actualizado);
        }

        // DELETE: api/sorteos/{sorteoId}/premios/{id}
        [HttpDelete("{sorteoId:int}/premios/{id:int}")]
        [SwaggerOperation(
           Summary = "Eliminacion de premios usando id sorteo y id premio"

       )]
        public async Task<IActionResult> Eliminar(int sorteoId, int id)
        {
            var eliminado = await _service.EliminarPremioAsync(sorteoId, id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }

    }
}
