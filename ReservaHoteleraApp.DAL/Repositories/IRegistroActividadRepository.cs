using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IRegistroActividadRepository : IRepository<RegistroActividad, RegistroActividadModel, RegistroActividadAdd, RegistroActividadUpdate, RegistroActividadRemove>
    {
        
    }
}
