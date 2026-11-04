using System;
using System.Collections.Generic;
using System.Text;
using ReservaHoteleraApp.DAL.Models.Base;
using ReservaHoteleraApp.DAL.Models;
using Microsoft.Extensions.Logging;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Base
{
    public abstract class ServiceBase
    {
        private readonly ILogger _logger;
        protected ServiceBase(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<OperationResult<T>> ExecuteAsync<T>(
        string operation, int? userId, Func<Task<OperationResult<T>>> action)
        {
            try
            {
                _logger.LogInformation("ejecutando {Operation}. Usuario: {UserId}", operation, userId);
                return await action();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar {Operation}. Usuario: {UserId}", operation, userId);

                return Failure<T>("Ocurrió un error inesperado. Intente nuevamente.");

            }
        }
        protected Task<OperationResult<bool>> DeleteEntityAsync<TRemove>(
            TRemove? remove, Func<TRemove, Task<OperationResult<bool>>> deleteAction, string entityName,
            Func<TRemove, string?>? validate = null)
            where TRemove : class, IRemoveRequest
        {
            return ExecuteAsync<bool>($"Eliminar {entityName}", remove?.ModifyUser, async () =>
            {
                if (remove is null)
                    return Failure<bool>($"El objeto {typeof(TRemove).Name} es requerido.");
                if (remove.Id <= 0)
                    return Failure<bool>($"El Id del objeto {typeof(TRemove).Name} es requerido y debe ser mayor a cero.");
                var validation = validate?.Invoke(remove);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await deleteAction(remove);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = $"Se eliminó {entityName} correctamente." }
                    : Failure<bool>(result.Message ?? $"No fue posible eliminar {entityName}.");
            });
        }
            


    }
}