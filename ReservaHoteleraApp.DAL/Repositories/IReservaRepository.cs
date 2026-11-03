using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IReservaRepository : IRepository<Reserva, ReservaModel, ReservaAdd, ReservaUpdate, ReservaRemove>
    {
       
    }
}
