using ReservaHoteleraApp.DataAccess.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DataAccess.Entities
{
    public sealed class Estadia : EstanciaEntity
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }

    }
}
