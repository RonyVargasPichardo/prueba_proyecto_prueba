using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Premialo.server.Infrastructure.SwaggerConfigs
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
            if (!type.IsEnum) return;

            // 1) Añadimos una descripción con el mapeo "int = Name (DisplayName)".
            var items = Enum.GetValues(type).Cast<object>();
            var mapping = items.Select(v =>
            {
                var name = Enum.GetName(type, v)!;
                var val = Convert.ToInt32(v);
                var fi = type.GetField(name);
                var display = fi?.GetCustomAttribute<DisplayAttribute>()?.Name;
                return $"{val} = {name}" + (display != null ? $" ({display})" : "");
            });

            schema.Description = (schema.Description ?? "") + $" Valores aceptados: <table> <tr><td>{string.Join("</td></tr> <tr><td>", mapping)} </td></tr> </table>";

            // 2) (opcional) añadimos una extensión x-enumNames con los nombres — útiles para algunos clientes.
            var arr = new OpenApiArray();
            foreach (var name in Enum.GetNames(type)) arr.Add(new OpenApiString(name));
            schema.Extensions["x-enumNames"] = arr;
        }
    }
}
