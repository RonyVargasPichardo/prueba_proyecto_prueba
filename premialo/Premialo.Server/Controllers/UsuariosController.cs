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
    [Route("api/admin/usuarios")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosService _service;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;
        public UsuariosController(UsuariosService servise, IConfiguration configuration)
        {
            _service = servise;
            _configuration = configuration;
        }




    }
}
