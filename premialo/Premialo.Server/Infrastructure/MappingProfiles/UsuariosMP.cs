using AutoMapper;
using Premialo.Server.DTOs.Usuarios;
using Premialo.Server.Models;

namespace Premialo.Server.Infrastructure.MappingProfiles
{
    public class UsuariosMP : Profile
    {
        public UsuariosMP()
        {
            CreateMap<Usuario, UsuarioDTO>();
        }
    }
}
