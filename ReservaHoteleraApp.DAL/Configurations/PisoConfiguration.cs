using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class PisoConfiguration : IEntityTypeConfiguration<Piso>
    {
        public void Configure(EntityTypeBuilder<Piso> builder)
        {
            builder.ToTable("Pisos");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Numero)
                .IsRequired();

            builder.Property(p => p.Descripcion)
                .HasMaxLength(250);

            builder.HasIndex(p => new { p.IdBloque, p.Numero })
                .IsUnique();

            builder.HasOne(p => p.Bloque)
                .WithMany(b => b.Pisos)
                .HasForeignKey(p => p.IdBloque)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
