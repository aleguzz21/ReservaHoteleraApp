using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services
{
    public class EstadiaServices : IEstadiaServices
    {
        private readonly IEstadiaRepository _repository;
        private readonly ILogger<EstadiaServices> _logger;

        public EstadiaServices(IEstadiaRepository repository, ILogger<EstadiaServices> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<OperationResult<List<EstadiaModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<EstadiaModel>>("Consultar estadías", null, async () =>
            {
                var result = await _repository.GetAllAsync();
                if (!result.Success || result.Data is null)
                    return Failure<List<EstadiaModel>>(result.Message ?? "No fue posible consultar las estadías.");

                var models = result.Data.Select(MapToModel).ToList();
                return new OperationResult<List<EstadiaModel>>
                {
                    Data = models,
                    Message = "Estadías consultadas correctamente."
                };
            });
        }

        public Task<OperationResult<EstadiaModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<EstadiaModel>("Consultar estadía", null, async () =>
            {
                if (id <= 0)
                    return Failure<EstadiaModel>("El identificador de la estadía debe ser mayor que cero.");

                var result = await _repository.GetByIdAsync(id);
                if (result is null || !result.Success || result.Data is null)
                    return Failure<EstadiaModel>("La estadía no existe.");

                return result;
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

        private async Task<OperationResult<T>> ExecuteAsync<T>(string operation, int? userId, Func<Task<OperationResult<T>>> action)
        {
            try
            {
                _logger.LogInformation("Ejecutando {Operation}. Usuario: {UserId}", operation, userId);
                return await action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar {Operation}. Usuario: {UserId}", operation, userId);
                return Failure<T>("Ocurrió un error inesperado. Intente nuevamente.");
            }
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

        private static EstadiaModel MapToModel(Estadia estadia)
        {
            return new EstadiaModel
            {
                Id = estadia.Id,
                IdReserva = estadia.IdReserva,
                IdUsuario = estadia.IdUsuario,
                FechaEntrada = estadia.FechaEntrada,
                FechaSalida = estadia.FechaSalida
            };
        }
    }
}
