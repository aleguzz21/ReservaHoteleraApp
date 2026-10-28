using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IUsuarioRepository
    {
        Task<OperationResult<List<Usuario>>> GetAllAsync();
        Task<OperationResult<UsuarioModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario);
        Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario);
        Task<OperationResult<bool>> DeleteAsync(UsuarioRemove usuario);
    }
}
