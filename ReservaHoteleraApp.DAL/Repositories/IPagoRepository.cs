using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Pago;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IPagoRepository : IRepository<Pago, PagoModel, PagoAdd, PagoUpdate, PagoRemove>
    {
   
    }
}
