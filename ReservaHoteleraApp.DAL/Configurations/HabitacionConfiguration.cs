using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class HabitacionConfiguration : IEntityTypeConfiguration<Habitacion>
    {
        public void Configure(EntityTypeBuilder<Habitacion> builder)
        {
            builder.ToTable("Habitaciones");
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Numero)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(h => h.PrecioNoche)
                .HasPrecision(18, 2);

            builder.Property(h => h.Estado)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Disponible");

            builder.HasIndex(h => h.Numero).IsUnique();

            builder.HasOne(h => h.TipoHabitacion)
                .WithMany(t => t.Habitaciones)
                .HasForeignKey(h => h.IdTipoHabitacion)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
