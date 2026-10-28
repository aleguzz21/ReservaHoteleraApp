using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.NombreUsuario)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Contrasena)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.Rol)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(u => u.Activo)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasIndex(u => u.NombreUsuario).IsUnique();
        }
    }
}
