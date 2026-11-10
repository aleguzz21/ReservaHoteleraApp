using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<ReservaRepository> logger;

        public ReservaRepository(HotelDbContext context, ILogger<ReservaRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<ReservaModel>>> GetAllAsync()
        {
            try
            {
                var reservas = await context.Reservas.AsNoTracking().ToListAsync();
                var models = reservas.Select(MapToModel).ToList();

                return new OperationResult<List<ReservaModel>>
                {
                    Data = models,
                    Message = "Reservas consultadas correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar las reservas.");
                return Failure<List<ReservaModel>>("No se pudieron consultar las reservas.");
            }
        }

        public async Task<OperationResult<ReservaModel>> GetByIdAsync(int id)
        {
            try
            {
                var reserva = await context.Reservas.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                if (reserva is null)
                    return Failure<ReservaModel>("La reserva no existe.");

                return new OperationResult<ReservaModel>
                {
                    Data = MapToModel(reserva),
                    Message = "Reserva consultada correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar la reserva con id {Id}.", id);
                return Failure<ReservaModel>("No se pudo consultar la reserva.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(ReservaAdd reserva)
        {
            if (reserva is null)
                return Failure<bool>("El objeto ReservaAdd es requerido.");

            var validation = ValidateReserva(reserva);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new Reserva
                {
                    IdCliente = reserva.IdCliente,
                    IdHabitacion = reserva.IdHabitacion,
                    CantidadPersonas = reserva.CantidadPersonas,
                    Estado = string.IsNullOrWhiteSpace(reserva.Estado) ? "Pendiente" : reserva.Estado.Trim(),
                    TotalReserva = reserva.TotalReserva,
                    FechaEntrada = reserva.FechaEntrada,
                    FechaSalida = reserva.FechaSalida,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = reserva.CreatedUser
                };

                await context.Reservas.AddAsync(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Reserva registrada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar la reserva.");
                return Failure<bool>("No se pudo registrar la reserva. Verifique que el cliente y la habitación existan.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar la reserva.");
                return Failure<bool>("Ocurrió un error inesperado al registrar la reserva.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(ReservaUpdate reserva)
        {
            if (reserva is null)
                return Failure<bool>("El objeto ReservaUpdate es requerido.");

            var validation = ValidateReserva(reserva);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.Reservas.FirstOrDefaultAsync(r => r.Id == reserva.Id);
                if (entity is null)
                    return Failure<bool>("La reserva no existe.");

                entity.IdCliente = reserva.IdCliente;
                entity.IdHabitacion = reserva.IdHabitacion;
                entity.CantidadPersonas = reserva.CantidadPersonas;
                entity.Estado = string.IsNullOrWhiteSpace(reserva.Estado) ? "Pendiente" : reserva.Estado.Trim();
                entity.TotalReserva = reserva.TotalReserva;
                entity.FechaEntrada = reserva.FechaEntrada;
                entity.FechaSalida = reserva.FechaSalida;
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = reserva.ModifyUser;

                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Reserva actualizada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar la reserva.");
                return Failure<bool>("No se pudo actualizar la reserva. Verifique que el cliente y la habitación existan.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar la reserva.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar la reserva.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(ReservaRemove reserva)
        {
            if (reserva is null)
                return Failure<bool>("El objeto ReservaRemove es requerido.");

            try
            {
                var entity = await context.Reservas.FirstOrDefaultAsync(r => r.Id == reserva.Id);
                if (entity is null)
                    return Failure<bool>("La reserva no existe.");

                context.Reservas.Remove(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Reserva eliminada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar la reserva.");
                return Failure<bool>("No se pudo eliminar la reserva porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar la reserva.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar la reserva.");
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
