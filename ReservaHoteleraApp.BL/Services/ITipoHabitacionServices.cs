using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;

namespace ReservaHoteleraApp.BL.Services
{
    public interface ITipoHabitacionServices
    {
        Task<OperationResult<List<TipoHabitacionModel>>> GetAllAsync();
        Task<OperationResult<TipoHabitacionModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(TipoHabitacionAdd? tipoHabitacion);
        Task<OperationResult<bool>> UpdateAsync(TipoHabitacionUpdate? tipoHabitacion);
    }
}
