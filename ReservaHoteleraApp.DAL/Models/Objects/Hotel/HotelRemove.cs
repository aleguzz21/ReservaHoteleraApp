using System;
using System.Collections.Generic;
using System.Text;
using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Objects.Hotel
{
    public class HotelRemove : IRemoveRequest
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
