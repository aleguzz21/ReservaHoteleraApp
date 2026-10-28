using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Pago;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IPagoRepository
    {
        Task<OperationResult<List<Pago>>> GetAllAsync();
        Task<OperationResult<PagoModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(PagoAdd pago);
        Task<OperationResult<bool>> UpdateAsync(PagoUpdate pago);
        Task<OperationResult<bool>> DeleteAsync(PagoRemove pago);
    }
}
