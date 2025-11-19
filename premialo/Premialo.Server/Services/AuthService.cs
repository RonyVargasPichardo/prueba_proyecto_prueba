using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Models;
using Premialo.Server.Models.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Premialo.Server.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        CurrentUserService _currentUser;
        private readonly int _intentos_login_maximos = 5;
        private readonly int _tiempo_bloqueo_minutos = 30;
        public AuthService(AppDbContext db, IConfiguration configuration, CurrentUserService currentUser, IMapper mapper)
        {
            _db = db;
            _configuration = configuration;
            _currentUser= currentUser;
            _mapper = mapper;

            int.TryParse(_configuration["Security:intentos_login_maximos"], out _intentos_login_maximos);
            int.TryParse(_configuration["Security:tiempo_bloqueo_minutos"], out _tiempo_bloqueo_minutos);
        }
        public async Task<ChangePasswordResultDTO> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            // Obtener ID del usuario autenticado
            var idUsuario = _currentUser.UserId;

            if (!IsNewPasswordValid(newPassword)) throw new InvalidDataException("La nueva contraseña no es válida, debe tener un mínimo de 8 caracteres, incluyendo mayúsculas, minúsculas y números");

            var usuario = await _db.Usuarios.FindAsync(idUsuario);

            if (usuario == null) throw new NullReferenceException("No se encontró el usuario en línea en la BD");

            // Verificar contraseña actual
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, usuario.PasswordHash))
                throw new ArgumentException("La contraseña actual es incorrecta");

            // Encriptar la nueva contraseña
            var nuevaHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            usuario.PasswordHash = nuevaHash;
            usuario.FechaPassword = DateTime.Now;
            usuario.MustChangePassword = false;
            await _db.SaveChangesAsync();

            return new ChangePasswordResultDTO(true, "Contraseña actualizada exitósamente");
        }

        public bool IsNewPasswordValid(string password)
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
            return regex.IsMatch(password);
        }

        public async Task<LoginResultDTO> LoginAsync(string username, string password)
        {
            username = username.ToLower();

            var result = _db.Usuarios.Where(u => u.UserName.ToLower() == username);

            var usuario = result.FirstOrDefault();

            //No se encuentra el usuario
            if (usuario == null)
                return new LoginResultDTO("Usuario o contraseña no validos");

            //Si "BloqueadoHasta" es mayor que ahora
            if (usuario.BloqueadoHasta != null && usuario.BloqueadoHasta > DateTime.Now)
                return new LoginResultDTO($"Usuario bloqueado hasta {((DateTime)usuario.BloqueadoHasta).ToString("dd/MM/yyyy hh:mm:ss tt")}");

            //Si los intentos de inicio de sesion superan los intentos máximos se bloquea el usuario 
            if (usuario.IntentosLogin > _intentos_login_maximos)
            {
                usuario.IntentosLogin = 0;
                usuario.BloqueadoHasta = DateTime.Now.AddMinutes(_tiempo_bloqueo_minutos);
                await _db.SaveChangesAsync();
                return new LoginResultDTO($"Usuario bloqueado hasta {((DateTime)usuario.BloqueadoHasta).ToString("dd/MM/yyyy hh:mm:ss tt")}");
            }

            //Contraseña inválida, se aumenta el conteo de intentos
            if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                usuario.IntentosLogin = usuario.IntentosLogin + 1;
                await _db.SaveChangesAsync();
                return new LoginResultDTO("Usuario o contraseña no validos");
            }

            if (usuario.Estatus == EstatusEnum.Inactivo)
                return new LoginResultDTO("Usuario no está activo");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.UserName),
                new Claim("RolId", ((int)usuario.Rol).ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
            };

            var jwtKey = _configuration["Jwt:Key"];
            var sessionDuration = _configuration["Jwt:sessionDuration"];
            int defaultSessionDuration = 1440;
            int.TryParse(sessionDuration, out defaultSessionDuration);
            if (string.IsNullOrEmpty(jwtKey)) throw new Exception("No se encontro la propiedad \"Jwt.Key\" el el archivo appsettings.json");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(defaultSessionDuration),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);

            //Actualizando informaciones de usuario tras login exitoso
            usuario.IntentosLogin = 0;
            usuario.UltimoAcceso = DateTime.Now;
            await _db.SaveChangesAsync();

            return new LoginResultDTO("Éxito al iniciar sesión", tokenString, usuarioDTO);
        }

        public async Task<UsuarioDTO> Me()
        {
            var idUsuario = _currentUser.UserId;

            var result = _db.Usuarios.Where(u => u.Id == idUsuario);

            return _mapper.Map<UsuarioDTO>(result.FirstOrDefault());
        }
    }
}
