using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;
using ReservaHoteleraApp.DAL.Models;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IUsuarioRepository : IRepository<UsuarioModel, UsuarioAdd, UsuarioUpdate, UsuarioRemove>
    {
        Task<OperationResult<UsuarioModel>> GetByNombreUsuarioAsync(string nombreUsuario);
    }
}
