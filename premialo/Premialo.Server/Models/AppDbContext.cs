using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Premialo.Server.DTOs.Asistencia;

namespace Premialo.Server.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Sorteo> Sorteos { get; set; }
        public DbSet<Premio> Premios { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<SorteoResultado> SorteosResultados { get; set; }

        public DbSet<Asistencia> Asistencias { get; set; }


        #region

        public DbSet<ConsultaCedulaDTO> ConsultaCedulaRDTOs { get; set; }
        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Data seeding
            //Semilla de usuarios
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    UserName = "admin",
                    Email = "admin@contraloria.gob.do",
                    Nombres = "Administrador",
                    Apellidos = "Sistema",
                    PasswordHash = "$2a$11$AfiMaYqby6eas.iVnYAdkO./0MRqoicDE8VrOqLgTisXL00Y9vFYy",
                    FechaCreacion = new DateTime(2025, 11, 12),
                    FechaModificacion = null,
                    FechaUltimoAcceso = null,
                    Rol = RolUsuario.Administrador,
                    Estatus = Enums.EstatusEnum.Activo,
                    IntentosLogin = 0,
                    FechaPassword = new DateTime(2025, 11, 12),
                    MustChangePassword = false,
                }
            );
            #endregion

            //Relaciones y configuraciones adicionales aquí
            #region Sorteos
            //Relación Sorteo - Premio con eliminación en cascada
            modelBuilder.Entity<Sorteo>()
                .HasMany(s => s.Premios)
                .WithOne(p => p.Sorteo)
                .HasForeignKey(p => p.SorteoId)
                .OnDelete(DeleteBehavior.Cascade);
            //Relación Sorteo - Participante con eliminación en cascada
            modelBuilder.Entity<Sorteo>()
                .HasMany(s => s.Participantes)
                .WithOne(p => p.Sorteo)
                .HasForeignKey(p => p.SorteoId)
                .OnDelete(DeleteBehavior.Cascade);
            //Relación Sorteo - SorteoResultado con eliminación en cascada
            modelBuilder.Entity<Sorteo>()
                .HasMany(s => s.Resultados)
                .WithOne(r => r.Sorteo)
                .HasForeignKey(r => r.SorteoId)
                .OnDelete(DeleteBehavior.Cascade);
            //Relación Sorteo - Usuario (creación y modificación) sin eliminación en cascada
            modelBuilder.Entity<Sorteo>()
                .HasOne(s => s.UsuarioCreacion)
                .WithMany(u => u.SorteosCreados)
                .HasForeignKey(s => s.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.NoAction);
            //Relación Sorteo - Usuario (modificación) sin eliminación en cascada
            modelBuilder.Entity<Sorteo>()
                .HasOne(s => s.UsuarioModificacion)
                .WithMany(u => u.SorteosEditados)
                .HasForeignKey(s => s.UsuarioModificacionId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Premios
            //Relación Premio - SorteoResultado con eliminación en cascada
            modelBuilder.Entity<Premio>()
                .HasOne(p => p.Resultado)
                .WithOne(r => r.Premio)
                .OnDelete(DeleteBehavior.Cascade);
            //Relación Premio - Usuario (creación) sin eliminación en cascada
            modelBuilder.Entity<Premio>()
                .HasOne(p => p.UsuarioCreacion)
                .WithMany(u => u.PremiosCreados)
                .HasForeignKey(p => p.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Participantes
            //Relación Participante - SorteoResultado con eliminación en cascada
            modelBuilder.Entity<Participante>()
                .HasOne(p => p.Resultado)
                .WithOne(r => r.Participante)
                .OnDelete(DeleteBehavior.Cascade);

            //Relación Participante - Usuario (creación) sin eliminación en cascada
            modelBuilder.Entity<Participante>()
                .HasOne(p => p.UsuarioCreacion)
                .WithMany(u => u.ParticipantesCreados)
                .HasForeignKey(p => p.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Resultados
            //Configuración adicional para SorteoResultado
            modelBuilder.Entity<SorteoResultado>()
                .HasOne(r => r.Premio)
                .WithOne(p => p.Resultado)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<SorteoResultado>()
                .HasOne(r => r.Participante)
                .WithOne(p => p.Resultado)
                .OnDelete(DeleteBehavior.NoAction);
            //Relación SorteoResultado - Usuario (creación) sin eliminación en cascada
            modelBuilder.Entity<SorteoResultado>()
                .HasOne(r => r.UsuarioCreacion)
                .WithMany(u => u.ResultadosCreados)
                .HasForeignKey(r => r.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region
            modelBuilder.Entity<Asistencia>()
                .HasOne(a=> a.UsuarioCreacion)
                .WithMany(u=> u.Asistencias)
                .HasForeignKey(a=> a.UsuarioCreacionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Asistencia>()
             .HasOne(a => a.Sorteo)
             .WithMany(u => u.Asistencias)
             .HasForeignKey(a => a.SorteoId)
             .OnDelete(DeleteBehavior.NoAction);

            #endregion


            ///storeprocedure de consulta de cedula
            #region
            modelBuilder.Entity<ConsultaCedulaDTO>()
               .HasNoKey() // Indica que no tiene clave primaria (obligatorio para SPs)
               .ToView(null); // Indica que no está mapeada a ninguna tabla/vista. ESTO EVITA EL CREATE TABLE.

            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
}
