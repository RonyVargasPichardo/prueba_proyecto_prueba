using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Premialo.server.Infrastructure.SwaggerConfigs
{
    public class OperationFilters : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            //operation.Parameters.Add(new OpenApiParameter
            //{
            //    Name = "X-Institucion-Codigo",
            //    In = ParameterLocation.Header,
            //    Description = "A custom header for demonstration",
            //    Required = true, // Set to true if the header is mandatory
            //    Schema = new OpenApiSchema { Type = "string" }
            //});
        }
    }
}
