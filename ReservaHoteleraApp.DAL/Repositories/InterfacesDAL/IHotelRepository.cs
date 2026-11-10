using System;
using System.Collections.Generic;
using System.Text;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IHotelRepository: IRepository<HotelModel, HotelAdd, HotelUpdate, HotelRemove>
    {

    }
}
