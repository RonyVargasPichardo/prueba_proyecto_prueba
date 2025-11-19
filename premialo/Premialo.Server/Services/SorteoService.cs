using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Attributes;
using Microsoft.OpenApi.Extensions;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Premios;
using Premialo.Server.DTOs.Sorteos;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Models;
using Premialo.Server.Models.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using static Premialo.Server.DTOs.Sorteos.SorteoTraerID;

namespace Premialo.Server.Services
{

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString());
            if (member != null && member.Length > 0)
            {
                var attrs = member[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DisplayAttribute)attrs[0]).Name;
                }
            }
            return enumValue.ToString();
        }
    }

    public class SorteoService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        CurrentUserService _currentUser;



        public SorteoService(AppDbContext db, IConfiguration configuration, CurrentUserService currentUser, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _currentUser = currentUser;
            _mapper = mapper;


        }

        public async Task<IEnumerable<SorteoTodosNameDto>> ObtenerTodosAsync()
        {
            var sorteos = await _db.Sorteos
                .Select(s => new SorteoTodosNameDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    FechaSorteo = s.FechaSorteo,
                    Descripcion = s.Descripcion,
                    Premios = s.Premios.Count(),
                    Participantes = s.Participantes.Count(),
                    Resultados = s.Resultados.Count()
                })
                .ToListAsync();

            return sorteos;
        }


        public async Task<SorteoSingleReadDTO?> GetSorteoSingle(int id)
        {
            var result = await _db.Sorteos.FindAsync(id);

            if (result == null) return null;

            return new SorteoSingleReadDTO()
            {
                Id = result.Id,
                Nombre = result.Nombre,
                Descripcion = result.Descripcion,
                FechaSorteo = result.FechaSorteo,
            };
        }


        public async Task<SorteoDetalleDto?> ObtenerPorIdAsync(int id)
        {
            var sorteo = await _db.Sorteos
        .Include(s => s.Resultados)
            .ThenInclude(r => r.Premio)
        .Include(s => s.Resultados)
            .ThenInclude(r => r.Participante)
        .FirstOrDefaultAsync(s => s.Id == id);

            if (sorteo == null)
                return null;
            var totalAsistencias = await _db.Asistencias
                .Where(a => a.SorteoId == id)
                .CountAsync();

            var totalParticipantes = await _db.Participantes
                .Where(p => p.SorteoId == id)
                .CountAsync();

            var totalParticipantesAsistieron = await _db.Asistencias
                .Where(a => a.SorteoId == id && a.participante == true)
                .CountAsync();

            var totalPremios = await _db.Premios
      .Where(p => p.SorteoId == id)
      .CountAsync();

        var premiosSorteados = await _db.Premios
    .Where(p => p.SorteoId == id && p.Estatus == EstatusPremioEnum.Sorteado)
    .CountAsync();

// 🔹 Premios pendientes (Estatus = Pendiente)
var premiosPendientes = await _db.Premios
    .Where(p => p.SorteoId == id && p.Estatus == EstatusPremioEnum.Pendiente)
    .CountAsync();

            return new SorteoDetalleDto
            {
                Id = sorteo.Id,
                Nombre = sorteo.Nombre,
                FechaSorteo = sorteo.FechaSorteo,
                Descripcion = sorteo.Descripcion,

                TotalAsistencias = totalAsistencias,
                TotalParticipantes = totalParticipantes,
                TotalParticipantesAsistieron = totalParticipantesAsistieron,
                TotalPremios = totalPremios,
                PremiosPendientes = premiosPendientes,
                PremiosSorteados = premiosSorteados,
                Resultados = sorteo.Resultados.Select(r => new ResultadoDto
                {
                    Idresultado = r.Id,
                    Fecha = r.Fecha,

                    Premio = new PremioDto
                    {
                        Idpremio = r.PremioId,
                        Nombre = r.Premio.Nombre,
                        Descripcion = r.Premio.Descripcion
                    },

                    Participante = new ParticipanteDto
                    {
                        Idparticipante = r.ParticipanteId,
                        DocumentoIdentidad = r.Participante.DocumentoIdentidad,
                        Nombres = r.Participante.Nombres,
                        Apellidos = r.Participante.Apellidos,
                        Cargo = r.Participante.Cargo,
                        UnidadOrganizativa = r.Participante.UnidadOrganizativa,
                        Estatus = ((ParticipanteEstatusEnum)r.Participante.Estatus).GetDisplayName()
                    }
                }).ToList()
            };
        }
   
        

        public async Task<Sorteo> CrearAsync(SorteoCreateDto dto)
        {
            var IdUsuarioOnline = _currentUser.UserId;


            var entidad = new Sorteo
            {
                Nombre = dto.Nombre,
                FechaSorteo = dto.FechaSorteo,
                Descripcion = dto.Descripcion,
                FechaCreacion = DateTime.Now,
                Estatus = (SorteoEstatusEnum)1,
                UsuarioCreacionId = IdUsuarioOnline,
            };

            _db.Sorteos.Add(entidad);
            await _db.SaveChangesAsync();

            return _mapper.Map<Sorteo>(entidad);
        }


        public async Task<Sorteo> EditarAsync(SorteoUpdateDto dto)
        {
            var usuarioId = _currentUser.UserId;

            var sorteo = await _db.Sorteos.FirstOrDefaultAsync(s => s.Id == dto.Id);
            if (sorteo == null)
                throw new Exception($"No se encontró el sorteo con ID {dto.Id}");

            sorteo.Nombre = dto.Nombre;
            sorteo.FechaSorteo = dto.FechaSorteo;
            sorteo.Descripcion = dto.Descripcion;  
            if (dto.Estatus.HasValue)
                sorteo.Estatus = dto.Estatus.Value;
            sorteo.FechaModificacion = DateTime.Now;
            sorteo.UsuarioModificacionId = usuarioId;

            _db.Sorteos.Update(sorteo);
            await _db.SaveChangesAsync();

            return sorteo;
        }


        public async Task<bool> EliminarAsync(int id)
        {
            var entidad = await _db.Sorteos.FindAsync(id);
            if (entidad == null)
                return false;

            _db.Sorteos.Remove(entidad);
            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<ProximoPremioDTO?> GetProximoPremioAsync(int sorteoId)
        {
            var premio = this.GetProximoPremioQuery(sorteoId);

            if (!premio.Any()) return null;

            return await premio.Select(p => new ProximoPremioDTO() 
            { 
                Id = p.Id,
                Descripcion = p.Descripcion,
                Nombre = p.Nombre,
                Orden = p.orden
            }).FirstAsync();
        }

        public async Task<ResultadoResponseDTO> Sortear(int sorteoId)
        {
            var participantesQuery = GetParticipantesValidosQuery(sorteoId);
            var proximoPremioQuery = GetProximoPremioQuery(sorteoId);

            var proximoPremio = await proximoPremioQuery.FirstOrDefaultAsync();

            if (proximoPremio == null) throw new Exception("No hay premios pendientes para sortear.");
            

            var participantesIds = await participantesQuery.Select(p => p.Id).ToArrayAsync();

            if (!participantesIds.Any()) throw new Exception("No hay participantes en estatus \"Concursando\" para sortear.");

            int indiceGanador = RandomNumberGenerator.GetInt32(0, participantesIds.Length);

            int participanteGanadorId = participantesIds[indiceGanador];

            var nuevoResultado = new SorteoResultado
            {
                SorteoId = sorteoId,
                PremioId = proximoPremio.Id,
                ParticipanteId = participanteGanadorId,
                Fecha = DateTime.Now,
                UsuarioCreacionId = _currentUser.UserId
            };

            _db.SorteosResultados.Add(nuevoResultado);

            proximoPremio.Estatus = EstatusPremioEnum.Sorteado;
            _db.Premios.Update(proximoPremio);

            var participanteGanador = await participantesQuery.FirstAsync(p => p.Id == participanteGanadorId);
            //Es una posibilidad poco probable
            if (participanteGanador == null) throw new NullReferenceException($"No se encontró la entidad {nameof(Participante)} con el ID: {participanteGanadorId} en la consulta {participantesQuery}.");

            participanteGanador.Estatus = ParticipanteEstatusEnum.Ganador;
            _db.Participantes.Update(participanteGanador);

            await _db.SaveChangesAsync();

            return new ResultadoResponseDTO
            {
                Id = nuevoResultado.Id,
                Fecha = nuevoResultado.Fecha,
                Premio = new PremioIfoDTO()
                {
                    Id = proximoPremio.Id,
                    Nombre = proximoPremio.Nombre,
                    Descripcion = proximoPremio.Descripcion,
                },
                Participante = new ParticipanteInfoDTO()
                {
                    Id = participanteGanadorId,
                    Nombres = participanteGanador.Nombres,
                    Apellidos = participanteGanador.Apellidos,
                    DocumentoIdentidad = participanteGanador.DocumentoIdentidad,
                    Cargo = participanteGanador.Cargo,
                    Unidad = participanteGanador.UnidadOrganizativa,
                }
            };
        }

        private IQueryable<Participante> GetParticipantesValidosQuery(int sorteoId)
        {
            return _db.Participantes
                .Where(p => p.SorteoId == sorteoId && p.Estatus == ParticipanteEstatusEnum.Concursando)
                .Include(p => p.Resultado)
                .Where(p => p.Resultado == null);
        }

        private IQueryable<Premio> GetProximoPremioQuery(int sorteoId)
        {
            return _db.Premios
                .Where(p => p.SorteoId == sorteoId && p.Estatus == EstatusPremioEnum.Pendiente)
                .Include(p => p.Resultado)
                .Where(p => p.Resultado == null)
                .OrderBy(p => p.orden)
                .Take(1);
        }
    }
}
