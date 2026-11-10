using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DAL.Models.Objects.Hotel
{
    public class HotelBase
    {
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Ciudad { get; set; }

    }
}
