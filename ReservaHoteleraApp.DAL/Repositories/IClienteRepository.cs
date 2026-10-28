using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using ReservaHoteleraApp.DAL.Models.Result;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IClienteRepository
    {
        Task<OperationResult<List<Cliente>>> GetAllAsync();
        Task<OperationResult<ClienteModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(ClienteAdd cliente);
        Task<OperationResult<bool>> UpdateAsync(ClienteUpdate cliente);
        Task<OperationResult<bool>> DeleteAsync(ClienteRemove cliente);
    }
}
