using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IUsuarioRepository : IRepository<UsuarioModel, UsuarioAdd, UsuarioUpdate, UsuarioRemove>
    {
    }
}
