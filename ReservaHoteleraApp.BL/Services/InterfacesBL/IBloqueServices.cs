using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Bloque;

namespace ReservaHoteleraApp.BL.Services.Interfaces
{
    public interface IBloqueServices
    {
        Task<OperationResult<List<BloqueModel>>> GetAllAsync();
        Task<OperationResult<BloqueModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(BloqueAdd? bloque);
        Task<OperationResult<bool>> UpdateAsync(BloqueUpdate? bloque);
        Task<OperationResult<bool>> DeleteAsync(BloqueRemove? bloque);
    }
}
