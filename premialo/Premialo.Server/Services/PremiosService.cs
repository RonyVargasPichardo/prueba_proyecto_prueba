using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Participantes;
using Premialo.Server.DTOs.Premios;
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
    public class PremiosService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        CurrentUserService _currentUser;

        public class PremioProfile : Profile
        {
            public PremioProfile()
            {
                CreateMap<Premio, PremioListadoDto>()
                    .ForMember(dest => dest.SorteoNombre,
                        opt => opt.MapFrom(src => src.Sorteo.Nombre))

                    .ForMember(dest => dest.UsuarioCrea,
                        opt => opt.MapFrom(src =>
                            src.UsuarioCreacion.Nombres + " " + src.UsuarioCreacion.Apellidos))

                    .ForMember(dest => dest.FechaResultado,
                        opt => opt.MapFrom(src =>
                            src.Resultado != null ?
                                src.Resultado.Fecha.ToString("yyyy-MM-dd hh:mm tt") :
                                ""))

                    .ForMember(dest => dest.GanadorDocumentoIdentidad,
                        opt => opt.MapFrom(src => src.Resultado != null ? src.Resultado.Participante.DocumentoIdentidad : ""))

                    .ForMember(dest => dest.GanadorNombres,
                        opt => opt.MapFrom(src => src.Resultado != null ? src.Resultado.Participante.Nombres : ""))

                    .ForMember(dest => dest.GanadorApellidos,
                        opt => opt.MapFrom(src => src.Resultado != null ? src.Resultado.Participante.Apellidos : ""))

                    .ForMember(dest => dest.GanadorCargo,
                        opt => opt.MapFrom(src => src.Resultado != null ? src.Resultado.Participante.Cargo : ""))

                    .ForMember(dest => dest.GanadorUnidadOrganizativa,
                        opt => opt.MapFrom(src => src.Resultado != null ? src.Resultado.Participante.UnidadOrganizativa : ""))

                    .ForMember(dest => dest.UsuarioSortea,
                        opt => opt.MapFrom(src =>
                            src.Resultado != null
                                ? src.Resultado.UsuarioCreacion.Nombres + " " + src.Resultado.UsuarioCreacion.Apellidos
                                : ""));
            }
        }



        public PremiosService(AppDbContext db, IConfiguration configuration, CurrentUserService currentUser, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _currentUser = currentUser;
            _mapper = mapper;

        }

        public async Task<IEnumerable<PremioListadoDto>> ObtenerPremiosAsync(
      int sorteoId, string? nombre)
        {
            var query = _db.Premios
                .Include(p => p.Sorteo)
                .Include(p => p.UsuarioCreacion)
                .Include(p => p.Resultado)
                    .ThenInclude(r => r.Participante)
                .Include(p => p.Resultado)
                    .ThenInclude(r => r.UsuarioCreacion)
                .Where(p => p.SorteoId == sorteoId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(p => p.Nombre.Contains(nombre));

            var premios = await query
                .OrderBy(p => p.orden)
                .ToListAsync();

            var lista = new List<PremioListadoDto>();

            foreach (var p in premios)
            {
                lista.Add(new PremioListadoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Orden = p.orden,
                    SorteoNombre = p.Sorteo?.Nombre ?? "",

                    Estatus = (int)p.Estatus,
                    EstatusNombre = ((EstatusPremioEnum)p.Estatus).GetDisplayName(),

                    FechaResultado = p.Resultado != null
                        ? p.Resultado.Fecha.ToString("yyyy-MM-dd hh:mm tt")
                        : "",

                    GanadorDocumentoIdentidad = p.Resultado?.Participante?.DocumentoIdentidad ?? "",
                    GanadorNombres = p.Resultado?.Participante?.Nombres ?? "",
                    GanadorApellidos = p.Resultado?.Participante?.Apellidos ?? "",
                    GanadorCargo = p.Resultado?.Participante?.Cargo ?? "",
                    GanadorUnidadOrganizativa = p.Resultado?.Participante?.UnidadOrganizativa ?? "",

                    UsuarioCrea = p.UsuarioCreacion != null
                        ? $"{p.UsuarioCreacion.Nombres} {p.UsuarioCreacion.Apellidos}"
                        : "",

                    UsuarioSortea = p.Resultado?.UsuarioCreacion != null
                        ? $"{p.Resultado.UsuarioCreacion.Nombres} {p.Resultado.UsuarioCreacion.Apellidos}"
                        : ""
                });
            }

            return lista;
        }


        public async Task<Premio> CrearPremioAsync(int sorteoId, PremiosCreateEditionDto dto)
        {
            var userId = _currentUser.UserId;

            var premio = new Premio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                orden = dto.Orden,

                RutaFoto = string.Empty,             // si luego agregas imagenes lo cambiamos
                FechaCreacion = DateTime.Now,

                SorteoId = sorteoId,
                UsuarioCreacionId = userId,

               Estatus = (EstatusPremioEnum) 1 // 1 = Activo
            };

            _db.Premios.Add(premio);
            await _db.SaveChangesAsync();

            return premio;
        }


        public async Task<Premio?> EditarPremioAsync(int sorteoId, int premioId, PremiosCreateEditionDto dto)
        {
            var premio = await _db.Premios
                .Where(p => p.SorteoId == sorteoId && p.Id == premioId)
                .FirstOrDefaultAsync();

            if (premio == null)
                return null;

            premio.Nombre = dto.Nombre;
            premio.Descripcion = dto.Descripcion;
            premio.orden = dto.Orden;

            await _db.SaveChangesAsync();
            return premio;
        }


        public async Task<bool> EliminarPremioAsync(int sorteoId, int premioId)
        {
            var premio = await _db.Premios
                .Where(p => p.SorteoId == sorteoId && p.Id == premioId)
                .FirstOrDefaultAsync();

            if (premio == null)
                return false;

            _db.Premios.Remove(premio);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<PremioListadoDto?> ObtenerPremioPorIdAsync(int sorteoId, int premioId)
        {
            var premio = await _db.Premios
                .Include(p => p.Sorteo)
                .Include(p => p.UsuarioCreacion)
                .Include(p => p.Resultado)
                    .ThenInclude(r => r.Participante)
                .Include(p => p.Resultado)
                    .ThenInclude(r => r.UsuarioCreacion)
                .Where(p => p.SorteoId == sorteoId && p.Id == premioId)
                .FirstOrDefaultAsync();

            if (premio == null)
                return null;

            var dto = new PremioListadoDto
            {
                Id = premio.Id,
                Nombre = premio.Nombre,
                Descripcion = premio.Descripcion,
                Orden = premio.orden,
                SorteoNombre = premio.Sorteo?.Nombre ?? "",

                FechaResultado = premio.Resultado != null
                    ? premio.Resultado.Fecha.ToString("yyyy-MM-dd hh:mm tt")
                    : "",

                GanadorDocumentoIdentidad = premio.Resultado?.Participante?.DocumentoIdentidad ?? "",
                GanadorNombres = premio.Resultado?.Participante?.Nombres ?? "",
                GanadorApellidos = premio.Resultado?.Participante?.Apellidos ?? "",
                GanadorCargo = premio.Resultado?.Participante?.Cargo ?? "",
                GanadorUnidadOrganizativa = premio.Resultado?.Participante?.UnidadOrganizativa ?? "",

                UsuarioCrea = premio.UsuarioCreacion != null
                    ? $"{premio.UsuarioCreacion.Nombres} {premio.UsuarioCreacion.Apellidos}"
                    : "",

                UsuarioSortea = premio.Resultado?.UsuarioCreacion != null
                    ? $"{premio.Resultado.UsuarioCreacion.Nombres} {premio.Resultado.UsuarioCreacion.Apellidos}"
                    : ""
            };

            return dto;
        }

        public async Task<int> CrearPremiosMasivosAsync(int sorteoId, PremioCargaMasivaDto dto)
        {
            var userId = _currentUser.UserId;

            var premios = dto.Premios.Select(p => new Premio
            {
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,

                orden = p.Orden,  

                FechaCreacion = DateTime.Now,
                RutaFoto = "premio.png",
                SorteoId = sorteoId,
                UsuarioCreacionId = userId,
                Estatus = EstatusPremioEnum.Pendiente
            }).ToList();

            await _db.Premios.AddRangeAsync(premios);
            await _db.SaveChangesAsync();

            return premios.Count;
        }
    }
}
