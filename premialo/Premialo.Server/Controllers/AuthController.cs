using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Premialo.server.DTOs.Auth;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Premialo.server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;
        public AuthController(AuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Iniciar sesión",
            Description = "**Descripción:** Permite a un usuario iniciar sesión en el sistema proporcionando sus credenciales."
        )]
        public async Task<ActionResult<LoginResultDTO>> Login(CredentialsDTO credentials)
        {
            var result = await _service.LoginAsync(credentials.Usuario, credentials.Contrasena);

            if(result.Success) return Ok(result);
            else return Unauthorized(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Cambiar contraseña",
            Description = "**Descripción:** Permite a un usuario autenticado cambiar su contraseña actual por una nueva."
        )]
        public async Task<ActionResult<ChangePasswordResultDTO>> ChangePassword(ChangePassWordRequestDTO req)
        {
            try
            {
                var result = await _service.ChangePasswordAsync(req.ContrasenaActual, req.NuevaContrasena);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex is InvalidDataException) ModelState.AddModelError(nameof(req.NuevaContrasena), ex.Message); 
                

                if (ex is ArgumentException) ModelState.AddModelError(nameof(req.ContrasenaActual), ex.Message);
                

                if (!ModelState.IsValid) return ValidationProblem(ModelState);

                throw;
            }
        }
        [HttpGet("Me")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Obtener información del usuario autenticado",
            Description = "**Descripción:** Obtiene los detalles del usuario actualmente autenticado. De acuerdo al Token (JWT) establecido."
        )]
        public async Task<ActionResult<UsuarioDTO>> Me()
        {
            return await _service.Me();
        }
    }
}
