using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IHabitacionRepository : IRepository<Habitacion, HabitacionModel, HabitacionAdd, HabitacionUpdate, HabitacionRemove>
    {
       
    }
}
