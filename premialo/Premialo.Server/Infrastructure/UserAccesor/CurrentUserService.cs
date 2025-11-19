using Premialo.Server.Models;
using System.Security.Claims;

public class CurrentUserService 
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId =>
        int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : 0;

    public string UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public int RoleId =>
        _httpContextAccessor.HttpContext?.User?.FindAll("RolId")
            .Select(c => int.TryParse(c.Value, out var id) ? id : 0)
            .Where(id => id != 0)
            .ToList().FirstOrDefault() ?? 0;

    public string Role =>
        _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList().FirstOrDefault() ?? "";

    public bool IsRoleId(int roleId) => RoleId == roleId;

    public bool IsAdmin => RoleId == (int)RolUsuario.Administrador;
}