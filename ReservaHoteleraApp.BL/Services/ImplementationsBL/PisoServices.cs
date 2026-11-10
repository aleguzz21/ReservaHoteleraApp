using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Piso;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    // Servicio de negocio del módulo Piso.
    // Valida los datos y luego delega el trabajo al repositorio (IPisoRepository).
    public class PisoServices : ServiceBase, IPisoServices
    {
        private readonly IPisoRepository _repository;

        public PisoServices(IPisoRepository repository, ILogger<PisoServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<PisoModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<PisoModel>>("Consultar pisos", null, async () =>
            {
                return await _repository.GetAllAsync();
            });
        }

        public Task<OperationResult<PisoModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<PisoModel>("Consultar piso", null, async () =>
            {
                if (id <= 0)
                    return Failure<PisoModel>("El identificador del piso debe ser mayor que cero.");
                return await _repository.GetByIdAsync(id);
            });
        }

        public Task<OperationResult<bool>> AddAsync(PisoAdd? piso)
        {
            return ExecuteAsync<bool>("Registrar piso", piso?.CreatedUser, async () =>
            {
                if (piso is null)
                    return Failure<bool>("El objeto PisoAdd es requerido.");

                var validation = ValidatePiso(piso);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(piso);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Piso registrado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar el piso.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(PisoUpdate? piso)
        {
            return ExecuteAsync<bool>("Actualizar piso", piso?.ModifyUser, async () =>
            {
                if (piso is null)
                    return Failure<bool>("El objeto PisoUpdate es requerido.");

                var validation = ValidatePiso(piso);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(piso);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Piso actualizado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar el piso.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(PisoRemove? piso)
        {
            return DeleteEntityAsync(piso, _repository.DeleteAsync, "el piso");
        }

        // Reglas de negocio del piso. Devuelve el mensaje de error, o null si todo está correcto.
        private static string? ValidatePiso(PisoBase piso)
        {
            if (piso.Numero <= 0)
                return "El número del piso debe ser mayor que cero.";
            if (piso.IdBloque <= 0)
                return "Debe seleccionar un bloque.";
            return null;
        }
    }
}
