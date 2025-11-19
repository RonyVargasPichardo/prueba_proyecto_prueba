using Microsoft.Identity.Client;
using Premialo.Server.DTOs.Usuarios;

namespace Premialo.server.DTOs.Auth
{
    public class LoginResultDTO: OperationResult<UsuarioDTO>
    {
        public LoginResultDTO(string errorMessage) : base(false, errorMessage) 
        { 
        }
        public LoginResultDTO(string successMessage, string token, UsuarioDTO user): base(true, successMessage, user)
        {
            Token = token;
        }
        public string? Token {get;set;}
    }
}
