using ReservaHoteleraApp.DAL.Models.Objects.Estadia;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IEstadiaRepository : IRepository<EstadiaModel, EstadiaAdd, EstadiaUpdate, EstadiaRemove>
    {
    }
}
