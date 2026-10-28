using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IEstadiaRepository
    {
        Task<OperationResult<List<Estadia>>> GetAllAsync();
        Task<OperationResult<EstadiaModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(EstadiaAdd estadia);
        Task<OperationResult<bool>> UpdateAsync(EstadiaUpdate estadia);
        Task<OperationResult<bool>> DeleteAsync(EstadiaRemove estadia);
    }
}
