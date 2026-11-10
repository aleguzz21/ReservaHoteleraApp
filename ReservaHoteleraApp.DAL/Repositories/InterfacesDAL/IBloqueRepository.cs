using ReservaHoteleraApp.DAL.Models.Objects.Bloque;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IBloqueRepository : IRepository<BloqueModel, BloqueAdd, BloqueUpdate, BloqueRemove>
    {
    }
}
