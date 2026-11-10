using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IHabitacionRepository : IRepository<HabitacionModel, HabitacionAdd, HabitacionUpdate, HabitacionRemove>
    {
    }
}
