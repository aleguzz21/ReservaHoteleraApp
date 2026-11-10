using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface ITipoHabitacionRepository : IRepository<TipoHabitacionModel, TipoHabitacionAdd, TipoHabitacionUpdate, TipoHabitacionRemove>
    {
    }
}
