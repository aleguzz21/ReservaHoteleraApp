using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class PagoConfiguration : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.ToTable("Pagos");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Monto)
                .HasPrecision(18, 2);

            builder.Property(p => p.MetodoPago)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Fecha)
                .IsRequired();

            builder.HasOne(p => p.Reserva)
                .WithMany(r => r.Pagos)
                .HasForeignKey(p => p.IdReserva)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
