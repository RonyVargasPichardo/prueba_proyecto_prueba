namespace Premialo.server.Infrastructure.SwaggerConfigs
{
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class AuthorizeRolesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Concat(context.MethodInfo.DeclaringType?
                    .GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>() ?? []);

            if (authorizeAttributes.Any())
            {
                var roles = string.Join(", ",
                    authorizeAttributes
                        .Where(a => !string.IsNullOrEmpty(a.Roles))
                        .Select(a => a.Roles)
                );

                if (!string.IsNullOrEmpty(roles))
                {
                    operation.Description +=
                        $"{(string.IsNullOrEmpty(operation.Description)? "" : "<br/><br/>")}**Autorización requerida:** Roles permitidos → `{roles}`";
                }
                else
                {
                    operation.Description += $"{(string.IsNullOrEmpty(operation.Description) ? "" : "<br/><br/>")}**Autorización requerida.**";
                }
            }
        }
    }

}
