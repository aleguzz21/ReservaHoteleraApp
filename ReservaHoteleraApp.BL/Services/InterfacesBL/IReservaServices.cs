using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;

namespace ReservaHoteleraApp.BL.Services.Interfaces
{
    public interface IReservaServices
    {
        Task<OperationResult<List<ReservaModel>>> GetAllAsync();
        Task<OperationResult<ReservaModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> EstaDisponibleAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida);
        Task<OperationResult<List<ReservaDetalleModel>>> GetByClienteIdAsync(int idCliente);
        Task<OperationResult<List<ReservaDetalleModel>>> GetAllConDetalleAsync();
        Task<OperationResult<bool>> CancelarAsync(int idReserva, int idCliente, bool esAdministrador);
        Task<OperationResult<bool>> AddAsync(ReservaAdd? reserva);
        Task<OperationResult<bool>> UpdateAsync(ReservaUpdate? reserva);
        Task<OperationResult<bool>> DeleteAsync(ReservaRemove? reserva);
    }
}
