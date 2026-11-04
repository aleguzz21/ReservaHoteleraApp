using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IReservaRepository : IRepository<Reserva, ReservaModel, ReservaAdd, ReservaUpdate, ReservaRemove>
    {
       
    }
}
