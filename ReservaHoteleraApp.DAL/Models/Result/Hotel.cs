using ReservaHoteleraApp.DAL.Models.Base;
using ReservaHoteleraApp.DAL.Models.Result;
using System;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class Hotel : AuditEntity
    { 
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Ciudad { get; set; }

        public ICollection<Bloque>Bloques { get; set; } = new List<Bloque>();

    }

}