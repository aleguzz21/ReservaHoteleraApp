using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;

namespace ReservaHoteleraApp.BL.Services
{
    public interface IRegistroActividadServices
    {
        Task<OperationResult<List<RegistroActividadModel>>> GetAllAsync();
        Task<OperationResult<RegistroActividadModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(RegistroActividadAdd? registroActividad);
        Task<OperationResult<bool>> UpdateAsync(RegistroActividadUpdate? registroActividad);
    }
}
