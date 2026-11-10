using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Repositories.BaseDAL
{
    public interface IRepository<TModel, TAdd, TUpdate, TRemove>
        where TAdd : class
        where TUpdate : class
        where TRemove : class, IRemoveRequest
    {
        Task<OperationResult<List<TModel>>> GetAllAsync();
        Task<OperationResult<TModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(TAdd add);
        Task<OperationResult<bool>> UpdateAsync(TUpdate update);
        Task<OperationResult<bool>> DeleteAsync(TRemove remove);
    }
}
