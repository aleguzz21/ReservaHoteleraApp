using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario, UsuarioModel, UsuarioAdd, UsuarioUpdate, UsuarioRemove>
    {
      
    }
}
