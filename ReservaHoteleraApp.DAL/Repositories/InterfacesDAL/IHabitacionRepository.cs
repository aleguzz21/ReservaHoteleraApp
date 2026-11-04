using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IHabitacionRepository : IRepository<Habitacion, HabitacionModel, HabitacionAdd, HabitacionUpdate, HabitacionRemove>
    {
       
    }
}
