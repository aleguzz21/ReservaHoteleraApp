using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Piso;

namespace ReservaHoteleraApp.BL.Services.Interfaces
{
    public interface IPisoServices
    {
        Task<OperationResult<List<PisoModel>>> GetAllAsync();
        Task<OperationResult<PisoModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(PisoAdd? piso);
        Task<OperationResult<bool>> UpdateAsync(PisoUpdate? piso);
        Task<OperationResult<bool>> DeleteAsync(PisoRemove? piso);
    }
}
