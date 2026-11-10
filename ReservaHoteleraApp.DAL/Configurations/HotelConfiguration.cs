using System;
using System.Collections.Generic;
using System.Text;
using ReservaHoteleraApp.DAL.Models.Base;
using ReservaHoteleraApp.DAL.Models.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReservaHoteleraApp.DAL.Configurations
{
    public sealed class HotelConfiguration : IEntityTypeConfiguration<Hotel> 
    {
        public void Configure(EntityTypeBuilder<Hotel> builder) {
            builder.ToTable("Hoteles");
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.Direccion)
                .HasMaxLength(200);

            builder.Property(h => h.Telefono)
                .HasMaxLength(20);

            builder.HasIndex(h => h.Nombre)
                .IsUnique();

            builder.Property(h => h.Ciudad)
                .HasMaxLength(100);
        }
    }
}
