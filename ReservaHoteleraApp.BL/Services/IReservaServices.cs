using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;

namespace ReservaHoteleraApp.BL.Services
{
    public interface IReservaServices
    {
        Task<OperationResult<List<ReservaModel>>> GetAllAsync();
        Task<OperationResult<ReservaModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(ReservaAdd? reserva);
        Task<OperationResult<bool>> UpdateAsync(ReservaUpdate? reserva);
        Task<OperationResult<bool>> DeleteAsync(ReservaRemove? reserva);
    }
}
