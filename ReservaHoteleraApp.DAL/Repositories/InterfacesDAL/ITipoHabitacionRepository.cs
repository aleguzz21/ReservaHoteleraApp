using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface ITipoHabitacionRepository : IRepository<TipoHabitacion, TipoHabitacionModel, TipoHabitacionAdd, TipoHabitacionUpdate, TipoHabitacionRemove>
    {
       
    }
}
