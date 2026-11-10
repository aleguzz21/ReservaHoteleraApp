using System;
using System.Collections.Generic;
using System.Text;
using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class Piso : AuditEntity
    {
        public int Numero { get; set; }
        public int IdBloque { get; set; }
        public string? Descripcion { get; set; }
        public Bloque? Bloque { get; set; }
        public ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();

    }
}
