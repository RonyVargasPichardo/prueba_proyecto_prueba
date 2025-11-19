using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Premialo.server.Infrastructure.SwaggerConfigs;
using Premialo.Server.Infrastructure.MappingProfiles;
using Premialo.Server.Models;
using Premialo.Server.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using static Premialo.Server.Services.ParticipantesService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAutoMapper(typeof(UsuariosMP));
builder.Services.AddAutoMapper(typeof(ParticipanteProfile));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CurrentUserService>();

//Agregar servicios 
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SorteoService>();
builder.Services.AddScoped<ParticipantesService>();
builder.Services.AddScoped<PremiosService>();
builder.Services.AddScoped<AsistenciaService>();


//Configuracion de la base de datos con interceptor para multi-tenant
var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrEmpty(connectionString)) throw new Exception("No se encontro la cadena de conexion \"Default\" el el archivo settings.json. ");
builder.Services.AddDbContext<AppDbContext>((serviceProvider, optionsBuilder) => optionsBuilder.UseSqlServer(connectionString));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var environment = builder.Environment.EnvironmentName;

        var jwtKey = builder.Configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey)) throw new Exception("No se encontro la propiedad \"Jwt.Key\" el el archivo settings.json");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = environment == "Production",
            ValidateAudience = environment == "Production",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "Prémialo API", Version = "v1" });
    config.OperationFilter<OperationFilters>();
    //Definiciones de seguridad para el JWT y el TenantId
    var sd = new SecurityDefinitions();
    foreach (var def in sd.definitions)
    {
        config.AddSecurityDefinition(def.SecuritySchemeId, def.SecurityScheme);
        config.AddSecurityRequirement(def.SecurityRequirement);
    }

    // Activa los atributos [SwaggerSchema], [SwaggerOperation], etc.
    config.EnableAnnotations();

    //// Incluir comentarios XML (para <summary> en propiedades/acciones)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) config.IncludeXmlComments(xmlPath); // mejora muchísimo la documentación. 

    //// Filtros (añade el EnumSchemaFilter y ValidationSchemaFilter que pongo más abajo)
    config.SchemaFilter<EnumSchemaFilter>();         // añade descripción de enums (mapa int -> nombre + Display)
    config.OperationFilter<AuthorizeRolesOperationFilter>(); // añade info de autorización en la descripción de las operaciones que tienen [Authorize]
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        //Los endpoints vienen collapsados por defecto (eso mejora la navegacion en la UI)
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true;
    });

    var builderSql = new SqlConnectionStringBuilder(connectionString);
    Console.WriteLine("Usando DB: {0} - {1}", builderSql.DataSource, builderSql.InitialCatalog);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
