using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    public class EstadiaServices : ServiceBase, IEstadiaServices
    {
        private readonly IEstadiaRepository _repository;

        public EstadiaServices(IEstadiaRepository repository, ILogger<EstadiaServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<EstadiaModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<EstadiaModel>>("Consultar estadías", null, async () =>
            {
                return await _repository.GetAllAsync();
            });
        }

        public Task<OperationResult<EstadiaModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<EstadiaModel>("Consultar estadía", null, async () =>
            {
                if (id <= 0)
                    return Failure<EstadiaModel>("El identificador de la estadía debe ser mayor que cero.");

                return await _repository.GetByIdAsync(id);
            });
        }

        public Task<OperationResult<bool>> AddAsync(EstadiaAdd? estadia)
        {
            return ExecuteAsync<bool>("Registrar estadía", estadia?.CreatedUser, async () =>
            {
                if (estadia is null)
                    return Failure<bool>("El objeto EstadiaAdd es requerido.");

                var validation = ValidateEstadia(estadia);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(estadia);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Estadía registrada correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar la estadía.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(EstadiaUpdate? estadia)
        {
            return ExecuteAsync<bool>("Actualizar estadía", estadia?.ModifyUser, async () =>
            {
                if (estadia is null)
                    return Failure<bool>("El objeto EstadiaUpdate es requerido.");

                var validation = ValidateEstadia(estadia);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(estadia);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Estadía actualizada correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar la estadía.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(EstadiaRemove? estadia)
        {
            return DeleteEntityAsync(estadia, _repository.DeleteAsync, "la estadía");
        }

        private static string? ValidateEstadia(EstadiaBase estadia)
        {
            if (estadia.IdReserva <= 0)
                return "El identificador de la reserva debe ser mayor que cero.";
            if (estadia.IdUsuario <= 0)
                return "El identificador del usuario debe ser mayor que cero.";
            if (estadia.FechaEntrada == default)
                return "La fecha de entrada es requerida.";
            if (estadia.FechaSalida.HasValue && estadia.FechaSalida.Value < estadia.FechaEntrada)
                return "La fecha de salida no puede ser anterior a la fecha de entrada.";

            return null;
        }
    }
}
