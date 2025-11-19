using Microsoft.OpenApi.Models;
using System.Xml.Linq;

namespace Premialo.server.Infrastructure.SwaggerConfigs
{
    public class SecurityDefinitions
    {
        public List<SecurityDefinition> definitions { get; set; }

        public SecurityDefinitions()
        {
            definitions = new List<SecurityDefinition>();
            //definitions.Add(new SecurityDefinition("X-Institucion-Codigo", new OpenApiSecurityScheme()
            //{
            //    Description = "Código que representa la institución",
            //    Name = "X-Institucion-Codigo",
            //    In = ParameterLocation.Header,
            //    Type = SecuritySchemeType.ApiKey,
            //    Scheme = "TenantIdScheme"
            //}, new OpenApiSecurityRequirement
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference
            //                {
            //                    Type = ReferenceType.SecurityScheme,
            //                    Id = "X-Institucion-Codigo"
            //                }
            //            },
            //            new string[] {}
            //        }
            //}));

            definitions.Add(new SecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Introduce el token JWT en este formato: Bearer {tu_token_aqui}"
            }, new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                }   
            ));
        }
    }
    public class SecurityDefinition
    {
        public OpenApiSecurityScheme SecurityScheme { get; set; }
        public OpenApiSecurityRequirement SecurityRequirement { get; set; }
        public string SecuritySchemeId { get; set; }
        public SecurityDefinition(string _SecuritySchemeId, OpenApiSecurityScheme _SecurityScheme, OpenApiSecurityRequirement _securityRequirement)
        {
            SecurityScheme = _SecurityScheme;
            SecuritySchemeId = _SecuritySchemeId;
            SecurityRequirement = _securityRequirement;
        }
    }
}
