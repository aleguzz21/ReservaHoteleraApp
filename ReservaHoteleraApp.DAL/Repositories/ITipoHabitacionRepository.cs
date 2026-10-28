using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface ITipoHabitacionRepository
    {
        Task<OperationResult<List<TipoHabitacion>>> GetAllAsync();
        Task<OperationResult<TipoHabitacionModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(TipoHabitacionAdd tipoHabitacion);
        Task<OperationResult<bool>> UpdateAsync(TipoHabitacionUpdate tipoHabitacion);
        Task<OperationResult<bool>> DeleteAsync(TipoHabitacionRemove tipoHabitacion);
    }
}
