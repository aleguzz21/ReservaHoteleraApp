using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;

namespace ReservaHoteleraApp.BL.Services.Interfaces
{
    public interface IHabitacionServices
    {
        Task<OperationResult<List<HabitacionModel>>> GetAllAsync();
        Task<OperationResult<HabitacionModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(HabitacionAdd? habitacion);
        Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate? habitacion);
        Task<OperationResult<bool>> DeleteAsync(HabitacionRemove? habitacion);
    }
}
