using ReservaHoteleraApp.DataAccess.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DataAccess.Entities
{
    public sealed class Reserva : EstanciaEntity
    {
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public int CantidadPersonas { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public decimal TotlalReserva { get; set; }

    }
}
