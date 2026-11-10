using System;
using System.Collections.Generic;
using System.Text;
using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class Bloque : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public int IdHotel { get; set; }
        public string? Descripcion { get; set; }
        public Hotel? Hotel { get; set; }
        public ICollection<Piso> Pisos { get; set; } = new List<Piso>();
    }
}
