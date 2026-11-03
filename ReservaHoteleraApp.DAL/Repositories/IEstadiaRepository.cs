using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IEstadiaRepository : IRepository<Estadia, EstadiaModel, EstadiaAdd, EstadiaUpdate, EstadiaRemove>
    {
  
    }
}
