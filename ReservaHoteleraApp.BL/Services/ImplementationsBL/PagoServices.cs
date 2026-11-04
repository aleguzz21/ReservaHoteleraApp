using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Pago;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    public class PagoServices : ServiceBase, IPagoServices
    {
        private readonly IPagoRepository _repository;

        public PagoServices(IPagoRepository repository, ILogger<PagoServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<PagoModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<PagoModel>>("Consultar pagos", null, async () =>
            {
                var result = await _repository.GetAllAsync();
                if (!result.Success || result.Data is null)
                    return Failure<List<PagoModel>>(result.Message ?? "No fue posible consultar los pagos.");

                var models = result.Data.Select(MapToModel).ToList();
                return new OperationResult<List<PagoModel>>
                {
                    Data = models,
                    Message = "Pagos consultados correctamente."
                };
            });
        }

        public Task<OperationResult<PagoModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<PagoModel>("Consultar pago", null, async () =>
            {
                if (id <= 0)
                    return Failure<PagoModel>("El identificador del pago debe ser mayor que cero.");

                var result = await _repository.GetByIdAsync(id);
                if (result is null || !result.Success || result.Data is null)
                    return Failure<PagoModel>("El pago no existe.");

                return result;
            });
        }

        public Task<OperationResult<bool>> AddAsync(PagoAdd? pago)
        {
            return ExecuteAsync<bool>("Registrar pago", pago?.CreatedUser, async () =>
            {
                if (pago is null)
                    return Failure<bool>("El objeto PagoAdd es requerido.");

                var validation = ValidatePago(pago);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(pago);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Pago registrado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar el pago.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(PagoUpdate? pago)
        {
            return ExecuteAsync<bool>("Actualizar pago", pago?.ModifyUser, async () =>
            {
                if (pago is null)
                    return Failure<bool>("El objeto PagoUpdate es requerido.");

                var validation = ValidatePago(pago);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(pago);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Pago actualizado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar el pago.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(PagoRemove? pago)
        {
            return DeleteEntityAsync(pago, _repository.DeleteAsync, "el pago");
        }

        private static string? ValidatePago(PagoBase pago)
        {
            if (pago.IdReserva <= 0)
                return "El identificador de la reserva debe ser mayor que cero.";
            if (pago.Monto <= 0)
                return "El monto debe ser mayor que cero.";
            if (string.IsNullOrWhiteSpace(pago.MetodoPago))
                return "El método de pago es requerido.";
            if (pago.MetodoPago.Trim().Length > 50)
                return "El método de pago no puede superar los 50 caracteres.";

            return null;
        }

        private static PagoModel MapToModel(Pago pago)
        {
            return new PagoModel
            {
                Id = pago.Id,
                IdReserva = pago.IdReserva,
                Monto = pago.Monto,
                Fecha = pago.Fecha,
                MetodoPago = pago.MetodoPago
            };
        }
    }
}
