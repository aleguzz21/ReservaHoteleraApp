using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IRegistroActividadRepository
    {
        Task<OperationResult<List<RegistroActividad>>> GetAllAsync();
        Task<OperationResult<RegistroActividadModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(RegistroActividadAdd registroActividad);
        Task<OperationResult<bool>> UpdateAsync(RegistroActividadUpdate registroActividad);
        Task<OperationResult<bool>> DeleteAsync(RegistroActividadRemove registroActividad);
    }
}
