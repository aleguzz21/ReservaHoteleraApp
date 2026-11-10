using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IReservaRepository : IRepository<ReservaModel, ReservaAdd, ReservaUpdate, ReservaRemove>
    {
    }
}
