using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Pago;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class PagoRepository : IPagoRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<PagoRepository> logger;

        public PagoRepository(HotelDbContext context, ILogger<PagoRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<PagoModel>>> GetAllAsync()
        {
            try
            {
                var pagos = await context.Pagos.AsNoTracking().ToListAsync();
                var models = pagos.Select(MapToModel).ToList();

                return new OperationResult<List<PagoModel>>
                {
                    Data = models,
                    Message = "Pagos consultados correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar los pagos.");
                return Failure<List<PagoModel>>("No se pudieron consultar los pagos.");
            }
        }

        public async Task<OperationResult<PagoModel>> GetByIdAsync(int id)
        {
            try
            {
                var pago = await context.Pagos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (pago is null)
                    return Failure<PagoModel>("El pago no existe.");

                return new OperationResult<PagoModel>
                {
                    Data = MapToModel(pago),
                    Message = "Pago consultado correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar el pago con id {Id}.", id);
                return Failure<PagoModel>("No se pudo consultar el pago.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(PagoAdd pago)
        {
            if (pago is null)
                return Failure<bool>("El objeto PagoAdd es requerido.");

            var validation = ValidatePago(pago);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new Pago
                {
                    IdReserva = pago.IdReserva,
                    Monto = pago.Monto,
                    Fecha = pago.Fecha == default ? DateTime.UtcNow : pago.Fecha,
                    MetodoPago = pago.MetodoPago!.Trim(),
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = pago.CreatedUser
                };

                await context.Pagos.AddAsync(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Pago registrado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar el pago.");
                return Failure<bool>("No se pudo registrar el pago. Verifique que la reserva exista.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar el pago.");
                return Failure<bool>("Ocurrió un error inesperado al registrar el pago.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(PagoUpdate pago)
        {
            if (pago is null)
                return Failure<bool>("El objeto PagoUpdate es requerido.");

            var validation = ValidatePago(pago);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.Pagos.FirstOrDefaultAsync(p => p.Id == pago.Id);
                if (entity is null)
                    return Failure<bool>("El pago no existe.");

                entity.IdReserva = pago.IdReserva;
                entity.Monto = pago.Monto;
                entity.Fecha = pago.Fecha == default ? DateTime.UtcNow : pago.Fecha;
                entity.MetodoPago = pago.MetodoPago!.Trim();
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = pago.ModifyUser;

                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Pago actualizado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar el pago.");
                return Failure<bool>("No se pudo actualizar el pago. Verifique que la reserva exista.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar el pago.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar el pago.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(PagoRemove pago)
        {
            if (pago is null)
                return Failure<bool>("El objeto PagoRemove es requerido.");

            try
            {
                var entity = await context.Pagos.FirstOrDefaultAsync(p => p.Id == pago.Id);
                if (entity is null)
                    return Failure<bool>("El pago no existe.");

                context.Pagos.Remove(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Pago eliminado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar el pago.");
                return Failure<bool>("No se pudo eliminar el pago porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar el pago.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar el pago.");
            }
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
