using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IClienteRepository : IRepository<ClienteModel, ClienteAdd, ClienteUpdate, ClienteRemove>
    {
    }
}
