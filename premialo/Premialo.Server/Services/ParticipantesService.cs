using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Participantes;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Models;
using Premialo.Server.Models.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static Premialo.Server.DTOs.Sorteos.SorteoTraerID;

namespace Premialo.Server.Services
{
    public class ParticipantesService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        CurrentUserService _currentUser;

        public ParticipantesService(AppDbContext db, IConfiguration configuration, CurrentUserService currentUser, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _currentUser = currentUser;
            _mapper = mapper;

        }
        public async Task<int> CrearParticipantesJsonAsync(
     int sorteoId,
     List<ParticipantesCreationDto> lista)
        {
            var userId = _currentUser.UserId;

            var participantes = lista.Select(p => new Participante
            {
                DocumentoIdentidad = p.DocumentoIdentidad,
                Nombres = p.Nombres,
                Apellidos = p.Apellidos,
                Email = p.Email,
                Telefono = p.Telefono,
                Cargo = p.Cargo,
                UnidadOrganizativa = p.UnidadOrganizativa,
                SorteoId = sorteoId,
                FechaRegistro = DateTime.Now,
                Estatus = (ParticipanteEstatusEnum)1,
                UsuarioCreacionId = userId
            }).ToList();

       
            await _db.Participantes.AddRangeAsync(participantes);
            await _db.SaveChangesAsync();

            return 1;
        }


        public async Task<ParticipantesDto?> ParticipantesIDAsync(int sorteoId, int participanteId)
        {
            var participante = await _db.Participantes
                .Include(p => p.Sorteo)
                .Include(p => p.UsuarioCreacion)
                .Include(p => p.Resultado)
                    .ThenInclude(r => r.Premio)
                .Where(p => p.SorteoId == sorteoId && p.Id == participanteId)
                .FirstOrDefaultAsync();

            if (participante == null)
                return null;

            var premio = participante.Resultado?.Premio;

            return new ParticipantesDto
            {
                Id = participante.Id,
                DocumentoIdentidad = participante.DocumentoIdentidad,
                Nombres = participante.Nombres,
                Apellidos = participante.Apellidos,
                Cargo = participante.Cargo,
                UnidadOrganizativa = participante.UnidadOrganizativa,
                FechaRegistro = participante.FechaRegistro.ToString("yyyy-MM-dd hh:mm tt"),
                Estatus = participante.Estatus,
                SorteoNombre = participante.Sorteo?.Nombre ?? "",
                PremioNombre = premio?.Nombre ?? "Sin premio",
                UsuarioCrea = participante.UsuarioCreacion.Nombres + " " + participante.UsuarioCreacion.Apellidos
            };
        }

        public async Task<IEnumerable<ParticipanteListFiltrosDto>> ParticipantesFiltros(
    int sorteoId,
    string? nombre,
    string? apellidos,
    string? documento,
    string? cargo,
    string? unidad)
        {
            var query = _db.Participantes
                .Where(p => p.SorteoId == sorteoId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(p => p.Nombres.Contains(nombre));

            if (!string.IsNullOrWhiteSpace(apellidos))
                query = query.Where(p => p.Apellidos.Contains(apellidos));

            if (!string.IsNullOrWhiteSpace(documento))
                query = query.Where(p => p.DocumentoIdentidad.Contains(documento));

            if (!string.IsNullOrWhiteSpace(cargo))
                query = query.Where(p => p.Cargo.Contains(cargo));

            if (!string.IsNullOrWhiteSpace(unidad))
                query = query.Where(p => p.UnidadOrganizativa.Contains(unidad));

            var lista = await query
                .OrderBy(p => p.Nombres)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ParticipanteListFiltrosDto>>(lista);
        }

        public async Task<bool> EliminarParticipantesAsync(int sorteoId, int participanteId)
        {
            var participante = await _db.Participantes
                .Where(p => p.SorteoId == sorteoId && p.Id == participanteId)
                .FirstOrDefaultAsync();

            if (participante == null)
                return false;

            _db.Participantes.Remove(participante);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<int> EliminarTodosParticipantesAsync(int sorteoId)
        {
            var participantes = await _db.Participantes
                .Where(p => p.SorteoId == sorteoId)
                .ToListAsync();

            if (!participantes.Any())
                return 0; 

            _db.Participantes.RemoveRange(participantes);
            await _db.SaveChangesAsync();

            return participantes.Count;
        }


        public class ParticipanteProfile : Profile
        {
            public ParticipanteProfile()
            {
                CreateMap<Participante, ParticipanteListFiltrosDto>()
                    .ForMember(dest => dest.Estatus,
                        opt => opt.MapFrom(src => src.Estatus.GetDisplayName()))
                    .ForMember(dest => dest.FechaRegistro,
                        opt => opt.MapFrom(src => src.FechaRegistro.ToString("yyyy-MM-dd hh:mm tt")));
            }
        }


    }
}
