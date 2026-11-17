using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    public class ReservaServices : ServiceBase, IReservaServices
    {
        private readonly IReservaRepository _repository;

        public ReservaServices(IReservaRepository repository, ILogger<ReservaServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<ReservaModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<ReservaModel>>("Consultar reservas", null, async () =>
            {
                return await _repository.GetAllAsync();
            });
        }

        public Task<OperationResult<ReservaModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<ReservaModel>("Consultar reserva", null, async () =>
            {
                if (id <= 0)
                    return Failure<ReservaModel>("El identificador de la reserva debe ser mayor que cero.");

                return await _repository.GetByIdAsync(id);
            });
        }
        public Task<OperationResult<bool>> EstaDisponibleAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida)
        {
            return ExecuteAsync<bool>("Verificar disponibilidad", null, async () =>
            {
                if (idHabitacion <= 0)
                    return Failure<bool>("El identificador de la habitación debe ser mayor que cero.");
                if (fechaEntrada < DateTime.Today)
                    return Failure<bool>("La fecha de entrada no puede ser anterior a hoy.");
                if (fechaSalida <= fechaEntrada)
                    return Failure<bool>("La fecha de salida debe ser posterior a la fecha de entrada.");

                return await _repository.EstaDisponibleAsync(idHabitacion, fechaEntrada, fechaSalida);
            });
        }

        public Task<OperationResult<List<ReservaDetalleModel>>> GetByClienteIdAsync(int idCliente)
        {
            return ExecuteAsync<List<ReservaDetalleModel>>("Consultar reservas del cliente", null, async () =>
            {
                if (idCliente <= 0)
                    return Failure<List<ReservaDetalleModel>>("El identificador del cliente debe ser mayor que cero.");

                return await _repository.GetByClienteIdAsync(idCliente);
            });
        }

        public Task<OperationResult<List<ReservaDetalleModel>>> GetAllConDetalleAsync()
        {
            return ExecuteAsync<List<ReservaDetalleModel>>("Consultar todas las reservas", null, async () =>
            {
                return await _repository.GetAllConDetalleAsync();
            });
        }

        public Task<OperationResult<bool>> CancelarAsync(int idReserva, int idCliente, bool esAdministrador)
        {
            return ExecuteAsync<bool>("Cancelar reserva", null, async () =>
            {
                if (idReserva <= 0)
                    return Failure<bool>("El identificador de la reserva debe ser mayor que cero.");

                var reserva = await _repository.GetByIdAsync(idReserva);
                if (!reserva.Success || reserva.Data is null)
                    return Failure<bool>(reserva.Message ?? "La reserva no existe.");

                if (!esAdministrador && reserva.Data.IdCliente != idCliente)
                    return Failure<bool>("No puedes cancelar una reserva que no es tuya.");

                if (reserva.Data.Estado == "Cancelada")
                    return Failure<bool>("La reserva ya está cancelada.");

                var actualizar = new ReservaUpdate
                {
                    Id = reserva.Data.Id,
                    IdCliente = reserva.Data.IdCliente,
                    IdHabitacion = reserva.Data.IdHabitacion,
                    CantidadPersonas = reserva.Data.CantidadPersonas,
                    Estado = "Cancelada",
                    TotalReserva = reserva.Data.TotalReserva,
                    FechaEntrada = reserva.Data.FechaEntrada,
                    FechaSalida = reserva.Data.FechaSalida
                };

                var resultado = await _repository.UpdateAsync(actualizar);
                return resultado.Success && resultado.Data
                    ? new OperationResult<bool> { Data = true, Message = "Reserva cancelada correctamente." }
                    : Failure<bool>(resultado.Message ?? "No se pudo cancelar la reserva.");
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

        public Task<OperationResult<bool>> DeleteAsync(ReservaRemove? reserva)
        {
            return DeleteEntityAsync(reserva, _repository.DeleteAsync, "la reserva");
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
    }
}
