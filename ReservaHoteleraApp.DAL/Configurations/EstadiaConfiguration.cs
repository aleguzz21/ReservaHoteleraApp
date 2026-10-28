using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class EstadiaConfiguration : IEntityTypeConfiguration<Estadia>
    {
        public void Configure(EntityTypeBuilder<Estadia> builder)
        {
            builder.ToTable("Estadias");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FechaEntrada)
                .IsRequired();

            builder.HasOne(e => e.Reserva)
                .WithOne(r => r.Estadia)
                .HasForeignKey<Estadia>(e => e.IdReserva)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Usuario)
                .WithMany(u => u.Estadias)
                .HasForeignKey(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
