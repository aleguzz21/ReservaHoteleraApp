using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IHabitacionRepository
    {
        Task<OperationResult<List<Habitacion>>> GetAllAsync();
        Task<OperationResult<HabitacionModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(HabitacionAdd habitacion);
        Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate habitacion);
        Task<OperationResult<bool>> DeleteAsync(HabitacionRemove habitacion);
    }
}
