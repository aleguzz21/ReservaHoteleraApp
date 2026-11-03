using System;
using System.Collections.Generic;
using System.Text;
using ReservaHoteleraApp.DAL.Models.Base;
using ReservaHoteleraApp.DAL.Models;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public interface IRepository<TEntity, TModel, TAdd, TUpdate, TRemove>
        where TAdd : class
        where TUpdate : class
        where TRemove : class, IRemoveRequest
    {
        Task<OperationResult<List<TEntity>>> GetAllAsync();
        Task<OperationResult<TModel>?> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(TAdd add);
        Task<OperationResult<bool>> UpdateAsync(TUpdate update);
        Task<OperationResult<bool>> DeleteAsync(TRemove remove);
    }
}
