using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Bloque;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    // Servicio de negocio del módulo Bloque.
    // Valida los datos y luego delega el trabajo al repositorio (IBloqueRepository).
    public class BloqueServices : ServiceBase, IBloqueServices
    {
        private readonly IBloqueRepository _repository;

        public BloqueServices(IBloqueRepository repository, ILogger<BloqueServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<BloqueModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<BloqueModel>>("Consultar bloques", null, async () =>
            {
                return await _repository.GetAllAsync();
            });
        }

        public Task<OperationResult<BloqueModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<BloqueModel>("Consultar bloque", null, async () =>
            {
                if (id <= 0)
                    return Failure<BloqueModel>("El identificador del bloque debe ser mayor que cero.");
                return await _repository.GetByIdAsync(id);
            });
        }

        public Task<OperationResult<bool>> AddAsync(BloqueAdd? bloque)
        {
            return ExecuteAsync<bool>("Registrar bloque", bloque?.CreatedUser, async () =>
            {
                if (bloque is null)
                    return Failure<bool>("El objeto BloqueAdd es requerido.");

                var validation = ValidateBloque(bloque);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(bloque);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Bloque registrado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar el bloque.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(BloqueUpdate? bloque)
        {
            return ExecuteAsync<bool>("Actualizar bloque", bloque?.ModifyUser, async () =>
            {
                if (bloque is null)
                    return Failure<bool>("El objeto BloqueUpdate es requerido.");

                var validation = ValidateBloque(bloque);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(bloque);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Bloque actualizado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar el bloque.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(BloqueRemove? bloque)
        {
            return DeleteEntityAsync(bloque, _repository.DeleteAsync, "el bloque");
        }

        // Reglas de negocio del bloque. Devuelve el mensaje de error, o null si todo está correcto.
        private static string? ValidateBloque(BloqueBase bloque)
        {
            if (string.IsNullOrWhiteSpace(bloque.Nombre))
                return "El nombre del bloque es requerido.";
            if (bloque.Nombre.Trim().Length > 100)
                return "El nombre del bloque no puede superar los 100 caracteres.";
            if (bloque.IdHotel <= 0)
                return "Debe seleccionar un hotel.";
            return null;
        }
    }
}
