using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IRegistroActividadRepository : IRepository<RegistroActividadModel, RegistroActividadAdd, RegistroActividadUpdate, RegistroActividadRemove>
    {
    }
}
