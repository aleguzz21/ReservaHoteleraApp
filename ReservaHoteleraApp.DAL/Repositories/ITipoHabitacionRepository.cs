using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface ITipoHabitacionRepository : IRepository<TipoHabitacion, TipoHabitacionModel, TipoHabitacionAdd, TipoHabitacionUpdate, TipoHabitacionRemove>
    {
       
    }
}
