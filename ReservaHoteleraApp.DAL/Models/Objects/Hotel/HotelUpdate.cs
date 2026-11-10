using System;
using System.Collections.Generic;
using System.Text;

namespace ReservaHoteleraApp.DAL.Models.Objects.Hotel
{
    public class HotelUpdate : HotelBase
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
