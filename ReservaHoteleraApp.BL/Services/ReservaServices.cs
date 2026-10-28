using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services
{
    public class ReservaServices : IReservaServices
    {
        private readonly IReservaRepository _repository;
        private readonly ILogger<ReservaServices> _logger;

        public ReservaServices(IReservaRepository repository, ILogger<ReservaServices> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<OperationResult<List<ReservaModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<ReservaModel>>("Consultar reservas", null, async () =>
            {
                var result = await _repository.GetAllAsync();
                if (!result.Success || result.Data is null)
                    return Failure<List<ReservaModel>>(result.Message ?? "No fue posible consultar las reservas.");

                var models = result.Data.Select(MapToModel).ToList();
                return new OperationResult<List<ReservaModel>>
                {
                    Data = models,
                    Message = "Reservas consultadas correctamente."
                };
            });
        }

        public Task<OperationResult<ReservaModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<ReservaModel>("Consultar reserva", null, async () =>
            {
                if (id <= 0)
                    return Failure<ReservaModel>("El identificador de la reserva debe ser mayor que cero.");

                var result = await _repository.GetByIdAsync(id);
                if (result is null || !result.Success || result.Data is null)
                    return Failure<ReservaModel>("La reserva no existe.");

                return result;
            });
        }

        public Task<OperationResult<bool>> AddAsync(ReservaAdd? reserva)
        {
            return ExecuteAsync<bool>("Registrar reserva", reserva?.CreatedUser, async () =>
            {
                if (reserva is null)
                    return Failure<bool>("El objeto ReservaAdd es requerido.");

                var validation = ValidateReserva(reserva);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(reserva);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Reserva registrada correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar la reserva.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(ReservaUpdate? reserva)
        {
            return ExecuteAsync<bool>("Actualizar reserva", reserva?.ModifyUser, async () =>
            {
                if (reserva is null)
                    return Failure<bool>("El objeto ReservaUpdate es requerido.");

                var validation = ValidateReserva(reserva);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(reserva);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Reserva actualizada correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar la reserva.");
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

        private static string? ValidateReserva(ReservaBase reserva)
        {
            if (reserva.IdCliente <= 0)
                return "El identificador del cliente debe ser mayor que cero.";
            if (reserva.IdHabitacion <= 0)
                return "El identificador de la habitación debe ser mayor que cero.";
            if (reserva.CantidadPersonas <= 0)
                return "La cantidad de personas debe ser mayor que cero.";
            if (!string.IsNullOrWhiteSpace(reserva.Estado) && reserva.Estado.Trim().Length > 20)
                return "El estado no puede superar los 20 caracteres.";
            if (reserva.TotalReserva < 0)
                return "El total de la reserva no puede ser negativo.";
            if (reserva.FechaEntrada == default)
                return "La fecha de entrada es requerida.";
            if (reserva.FechaSalida.HasValue && reserva.FechaSalida.Value < reserva.FechaEntrada)
                return "La fecha de salida no puede ser anterior a la fecha de entrada.";

            return null;
        }

        private static ReservaModel MapToModel(Reserva reserva)
        {
            return new ReservaModel
            {
                Id = reserva.Id,
                IdCliente = reserva.IdCliente,
                IdHabitacion = reserva.IdHabitacion,
                CantidadPersonas = reserva.CantidadPersonas,
                Estado = reserva.Estado,
                TotalReserva = reserva.TotalReserva,
                FechaEntrada = reserva.FechaEntrada,
                FechaSalida = reserva.FechaSalida
            };
        }
    }
}
