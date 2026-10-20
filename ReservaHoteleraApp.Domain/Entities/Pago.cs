using ReservaHoteleraApp.DataAccess.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DataAccess.Entities
{
    public sealed class Pago : IdEntity
    {
        public int IdReserva { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string MetodoPago { get; set; } = string.Empty;

    }
}
