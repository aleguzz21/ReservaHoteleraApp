using ReservaHoteleraApp.DAL.Models.Objects.Piso;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IPisoRepository : IRepository<PisoModel, PisoAdd, PisoUpdate, PisoRemove>
    {
    }
}
