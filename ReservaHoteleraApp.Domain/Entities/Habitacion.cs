using ReservaHoteleraApp.DataAccess.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DataAccess.Entities
{
    public sealed class Habitacion : IdEntity
    {
        public string Numero { get; set; } = string.Empty;
        public int IdTipoHabitacion { get; set; }
        public decimal PrecioNoche { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
