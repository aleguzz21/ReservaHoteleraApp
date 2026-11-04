using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IEstadiaRepository : IRepository<Estadia, EstadiaModel, EstadiaAdd, EstadiaUpdate, EstadiaRemove>
    {
  
    }
}
