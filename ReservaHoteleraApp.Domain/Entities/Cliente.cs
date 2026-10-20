using ReservaHoteleraApp.DataAccess.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DataAccess.Entities
{
    public sealed class Cliente : IdEntity
    {
        public int IdUsuario { get; set; }
        public string NombreCliente { get; set; }
        public string Documento { get; set; }
        public string Correo { get; set; }
        public string? Telefono { get; set; }
    
    }
}
