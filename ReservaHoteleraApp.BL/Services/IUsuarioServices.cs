using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;

namespace ReservaHoteleraApp.BL.Services
{
    public interface IUsuarioServices
    {
        Task<OperationResult<List<UsuarioModel>>> GetAllAsync();
        Task<OperationResult<UsuarioModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(UsuarioAdd? usuario);
        Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate? usuario);
    }
}
