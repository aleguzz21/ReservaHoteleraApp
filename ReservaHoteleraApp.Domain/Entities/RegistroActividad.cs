using ReservaHoteleraApp.DataAccess.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DataAccess.Entities
{
    public sealed class RegistroActividad :IdEntity
    {
        public int IdUsuario { get; set; }
        public string Accion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
    }
}
