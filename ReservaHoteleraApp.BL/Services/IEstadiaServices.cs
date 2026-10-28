using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;

namespace ReservaHoteleraApp.BL.Services
{
    public interface IEstadiaServices
    {
        Task<OperationResult<List<EstadiaModel>>> GetAllAsync();
        Task<OperationResult<EstadiaModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(EstadiaAdd? estadia);
        Task<OperationResult<bool>> UpdateAsync(EstadiaUpdate? estadia);
    }
}
