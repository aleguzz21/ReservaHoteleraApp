using ReservaHoteleraApp.DataAccess.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DataAccess.Entities
{
    public sealed class TipoHabitacion: IdEntity
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Capacidad { get; set; }
        public decimal Precio { get; set; }

    }
}
