using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IReservaRepository
    {
        Task<OperationResult<List<Reserva>>> GetAllAsync();
        Task<OperationResult<ReservaModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(ReservaAdd reserva);
        Task<OperationResult<bool>> UpdateAsync(ReservaUpdate reserva);
        Task<OperationResult<bool>> DeleteAsync(ReservaRemove reserva);
    }
}
