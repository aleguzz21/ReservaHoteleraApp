using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.ToTable("Reservas");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.CantidadPersonas)
                .IsRequired();

            builder.Property(r => r.Estado)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");

            builder.Property(r => r.TotalReserva)
                .HasPrecision(18, 2);

            builder.Property(r => r.FechaEntrada)
                .IsRequired();

            builder.HasIndex(r => new { r.IdHabitacion, r.FechaEntrada });

            builder.HasOne(r => r.Cliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Habitacion)
                .WithMany(h => h.Reservas)
                .HasForeignKey(r => r.IdHabitacion)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
