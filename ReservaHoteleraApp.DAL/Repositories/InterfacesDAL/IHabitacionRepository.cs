using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IHabitacionRepository : IRepository<HabitacionModel, HabitacionAdd, HabitacionUpdate, HabitacionRemove>
    {
        Task<OperationResult<List<HabitacionModel>>> GetDisponiblesAsync(DateTime fechaEntrada, DateTime fechaSalida);
    }
}
