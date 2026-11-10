using ReservaHoteleraApp.DAL.Models.Objects.Pago;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IPagoRepository : IRepository<PagoModel, PagoAdd, PagoUpdate, PagoRemove>
    {
    }
}
