using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class BloqueConfiguration :IEntityTypeConfiguration<Bloque>
    {
        public void Configure(EntityTypeBuilder<Bloque> builder) 
        { 
            builder.ToTable("Bloques");
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Descripcion)
                .HasMaxLength(250);

            builder.HasIndex(b => new { b.IdHotel, b.Nombre })
                .IsUnique();

            builder.HasOne(b=> b.Hotel)
                .WithMany(h => h.Bloques)
                .HasForeignKey(b => b.IdHotel)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
