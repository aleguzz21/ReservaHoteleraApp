using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class TipoHabitacionConfiguration : IEntityTypeConfiguration<TipoHabitacion>
    {
        public void Configure(EntityTypeBuilder<TipoHabitacion> builder)
        {
            builder.ToTable("TiposHabitacion");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Descripcion)
                .HasMaxLength(300);

            builder.Property(t => t.Capacidad)
                .IsRequired();

            builder.Property(t => t.Precio)
                .HasPrecision(18, 2);

            builder.HasIndex(t => t.Nombre).IsUnique();
        }
    }
}
