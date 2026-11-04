using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IRegistroActividadRepository : IRepository<RegistroActividad, RegistroActividadModel, RegistroActividadAdd, RegistroActividadUpdate, RegistroActividadRemove>
    {
        
    }
}
