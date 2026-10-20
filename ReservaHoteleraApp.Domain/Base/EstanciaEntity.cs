using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DataAccess.Base
{
    public abstract class EstanciaEntity : IdEntity
    {
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
    }
}
