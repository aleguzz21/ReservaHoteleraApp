using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IClienteRepository : IRepository<Cliente, ClienteModel, ClienteAdd, ClienteUpdate, ClienteRemove>
    {
   
    }
}
