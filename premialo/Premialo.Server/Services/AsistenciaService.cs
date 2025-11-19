using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Asistencia;
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
    public class AsistenciaService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        CurrentUserService _currentUser;

        
        public AsistenciaService(AppDbContext db, IConfiguration configuration, CurrentUserService currentUser, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _currentUser = currentUser;
            _mapper = mapper;

        }

        public async Task<ConsultaCedulaDTO?> ConsultarCedulaAsync(string documento )
        {
          
            var query = _db.ConsultaCedulaRDTOs
                .FromSqlInterpolated($"EXEC [dbo].[sp_Consulta_Cedula_HHRR] @Documento={documento}");

          
            var resultado = await Task.Run(() => query
                .AsEnumerable() 
                .FirstOrDefault()); 
           

       
            var todosLosResultados = await query.ToListAsync();
            var resultadoFinal = todosLosResultados.FirstOrDefault();

            return resultadoFinal;
        }

        public async Task<(int status, string mensaje)> RegistrarAsistenciaAsync(int sorteoId, string cedula)
        {
            var IdUsuarioOnline = _currentUser.UserId;

            var yaRegistrado = await _db.Asistencias
                .AnyAsync(a => a.SorteoId == sorteoId && a.Cedula == cedula);

            if (yaRegistrado)
                return (409, "La cédula ya fue registrada en asistencia.");

            var participante = await _db.Participantes
                .Where(p => p.SorteoId == sorteoId && p.DocumentoIdentidad == cedula)
                .FirstOrDefaultAsync();

            if (participante != null)
            {
                participante.Estatus = ParticipanteEstatusEnum.Concursando;
                var asistencia = new Asistencia
                {
                    Cedula = cedula,
                    Nombres = participante.Nombres,
                    Apellidos = participante.Apellidos,
                    SorteoId = sorteoId,
                    Fecha = DateTime.Now,
                    participante = true,
                    UsuarioCreacionId = IdUsuarioOnline
                };

                _db.Asistencias.Add(asistencia);
                await _db.SaveChangesAsync();

                return (200, "Participante encontrado. Marcado como Concursando y asistencia registrada.");
            }
            var consulta = await ConsultarCedulaAsync(cedula);

            if (consulta == null)
            {
                return (404, "La cédula no existe en participantes ni se encontraron datos externos.");
            }

            var nuevaAsistencia = new Asistencia
            {
                Cedula = cedula,
                Nombres = consulta.Nombres,
                Apellidos = consulta.Apellidos,
                SorteoId = sorteoId,
                Fecha = DateTime.Now,
                participante = false,
                UsuarioCreacionId = IdUsuarioOnline
            };

            _db.Asistencias.Add(nuevaAsistencia);
            await _db.SaveChangesAsync();

            return (200, "Cédula no pertenece al sorteo, pero asistencia registrada con datos externos.");
        }

        public async Task<AsistenciaDashboardDto> ObtenerAsistenciasAsync(int sorteoId)
        {
            
            var totalAsistencias = await _db.Asistencias
                .Where(a => a.SorteoId == sorteoId)
                .CountAsync();

          
            var totalParticipantes = await _db.Participantes
                .Where(p => p.SorteoId == sorteoId)
                .CountAsync();

            var totalAsistenciasParticipantes = await _db.Asistencias
                .Where(a => a.SorteoId == sorteoId)
                .Where(a=> _db.Participantes
                .Any(p=> p.DocumentoIdentidad == a.Cedula))
                .CountAsync();
       
            var totalNoParticipantes = await _db.Asistencias
                .Where(a => a.SorteoId == sorteoId)
                .Where(a => !_db.Participantes
                    .Any(p => p.SorteoId == sorteoId && p.DocumentoIdentidad == a.Cedula))
                .CountAsync();

            var asistencias = await _db.Asistencias
                .Where(a => a.SorteoId == sorteoId)
                .OrderBy(a => a.Nombres)
                .Select(a => new AsistenciaListadoDto
                {
                    Cedula = a.Cedula,
                    Nombres = a.Nombres,
                    Apellidos = a.Apellidos
                })
                .ToListAsync();

            return new AsistenciaDashboardDto
            {
                TotalAsistencias = totalAsistencias,
                TotalParticipantes = totalParticipantes,
                TotalAsistenciasParticipantes = totalAsistenciasParticipantes,
                TotalNoParticipantes = totalNoParticipantes,
                Asistencias = asistencias
            };
        }

    }
}
