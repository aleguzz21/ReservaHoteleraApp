using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;

namespace ReservaHoteleraApp.BL.Services
{
    public interface IClienteServices
    {
        Task<OperationResult<List<ClienteModel>>> GetAllAsync();
        Task<OperationResult<ClienteModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(ClienteAdd? cliente);
        Task<OperationResult<bool>> UpdateAsync(ClienteUpdate? cliente);
    }
}
