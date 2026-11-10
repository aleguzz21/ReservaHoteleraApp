using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Models.Base;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<TipoHabitacion> TiposHabitacion => Set<TipoHabitacion>();
        public DbSet<Habitacion> Habitaciones => Set<Habitacion>();
        public DbSet<Hotel> Hoteles => Set<Hotel>();
        public DbSet<Bloque> Bloques => Set<Bloque>();
        public DbSet<Piso> Pisos => Set<Piso>();

        public DbSet<Reserva> Reservas => Set<Reserva>();
        public DbSet<Estadia> Estadias => Set<Estadia>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<RegistroActividad> RegistrosActividad => Set<RegistroActividad>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HotelDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(AuditEntity.CreatedDate))
                        .HasDefaultValueSql("GETUTCDATE()");
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
