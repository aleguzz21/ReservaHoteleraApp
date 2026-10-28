using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class RegistroActividadConfiguration : IEntityTypeConfiguration<RegistroActividad>
    {
        public void Configure(EntityTypeBuilder<RegistroActividad> builder)
        {
            builder.ToTable("RegistrosActividad");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Accion)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(r => r.FechaHora)
                .IsRequired();

            builder.HasOne(r => r.Usuario)
                .WithMany(u => u.RegistrosActividad)
                .HasForeignKey(r => r.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
