using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Asistencia;
using Premialo.Server.DTOs.Participantes;
using Premialo.Server.DTOs.Premios;
using Premialo.Server.DTOs.Sorteos;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Models;
using Premialo.Server.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.Linq.Expressions;
using static Premialo.Server.DTOs.Sorteos.SorteoTraerID;

namespace Premialo.server.Controllers
{
    [Route("api/sorteos")]
    [ApiController]
    [Authorize]
    public class AsistenciaController : ControllerBase
    {
        private readonly AsistenciaService _service;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public AsistenciaController(AsistenciaService servise, IConfiguration configuration)
        {
            _service = servise;
            _configuration = configuration;
        }


        [HttpGet("{documento}/asistencia/cedulas")]
        [SwaggerOperation(
            Summary = "Consulta de cedula en HHRR",
            Description = "**Descripción:** busca la cedula en recursos humanos para validar existencia."
        )]
        public async Task<ActionResult<IEnumerable<ConsultaCedulaDTO>>> ConsultarCedula(string documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
            {
                return BadRequest("El número de documento es requerido.");
            }
            var resultado = await _service.ConsultarCedulaAsync(documento);

            if (resultado == null)
            {
                return NotFound($"No se encontró un empleado con el documento: {documento}");
            }

            return Ok(resultado);
        }

        [HttpPost("{sorteoId}/asistencia")]
        [SwaggerOperation(
            Summary = "Registro de asistencia mediante la cedula",
            Description = "**Descripción:** busca en participantes y si el participante exsiste cambia el estado a 2 (concursando) luego guarda en asistencia, si no es participante como sea guarda su asistencia en la tabla."
        )]
        public async Task<ActionResult<IEnumerable<AsistenciaCreateDto>>> Registrar(int sorteoId, [FromBody] AsistenciaCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Cedula))
                return BadRequest("Debe enviar una cédula válida.");

            var (status, mensaje) = await _service.RegistrarAsistenciaAsync(sorteoId, dto.Cedula);

            return status switch
            {
                200 => Ok(mensaje),
                409 => Conflict(mensaje),
                404 => NotFound(mensaje),
                _ => StatusCode(500, "Error desconocido.")
            };
        }

        [HttpGet("{sorteoId}/asistencia")]
        [SwaggerOperation(
            Summary = "Nos trae la cantidad total de asistencia de asistentes de forma individual ",
            Description = "**Descripción:** nos trae la cantidad total de asistentes al evento junto con la cantidad de participantes que asistieron y la cantidad de no participantes que asistieron ademas de un contero individual de cada uno."
        )]
        public async Task<ActionResult<IEnumerable<AsistenciaDashboardDto>>> Obtener(int sorteoId)
        {
            var result = await _service.ObtenerAsistenciasAsync(sorteoId);
            return Ok(result);
        }



    }
}
