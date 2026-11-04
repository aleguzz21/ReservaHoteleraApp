using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IClienteRepository : IRepository<Cliente, ClienteModel, ClienteAdd, ClienteUpdate, ClienteRemove>
    {
   
    }
}
