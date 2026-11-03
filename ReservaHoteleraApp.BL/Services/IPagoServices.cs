using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Pago;

namespace ReservaHoteleraApp.BL.Services
{
    public interface IPagoServices
    {
        Task<OperationResult<List<PagoModel>>> GetAllAsync();
        Task<OperationResult<PagoModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(PagoAdd? pago);
        Task<OperationResult<bool>> UpdateAsync(PagoUpdate? pago);
        Task<OperationResult<bool>> DeleteAsync(PagoRemove? pago);
    }
}
