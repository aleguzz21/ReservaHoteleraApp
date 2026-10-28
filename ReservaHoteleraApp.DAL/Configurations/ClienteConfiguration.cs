using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.NombreCliente)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Documento)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(c => c.Correo)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Telefono)
                .HasMaxLength(25);

            builder.HasIndex(c => c.Documento).IsUnique();
            builder.HasIndex(c => c.Correo).IsUnique();

            builder.HasOne(c => c.Usuario)
                .WithOne(u => u.Cliente)
                .HasForeignKey<Cliente>(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
