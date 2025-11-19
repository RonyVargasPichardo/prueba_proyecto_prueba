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
    public class UsuariosService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        CurrentUserService _currentUser;

        
        public UsuariosService(AppDbContext db, IConfiguration configuration, CurrentUserService currentUser, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _currentUser = currentUser;
            _mapper = mapper;

        }

        

    }
}
