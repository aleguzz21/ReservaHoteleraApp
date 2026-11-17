using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;
using ReservaHoteleraApp.DAL.Models;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IReservaRepository : IRepository<ReservaModel, ReservaAdd, ReservaUpdate, ReservaRemove>
    {
        Task<OperationResult<bool>> EstaDisponibleAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida);
        Task<OperationResult<List<ReservaDetalleModel>>> GetByClienteIdAsync(int idCliente);
        Task<OperationResult<List<ReservaDetalleModel>>> GetAllConDetalleAsync();
    }
}
