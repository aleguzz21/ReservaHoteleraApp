using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class EstadiaRepository : IEstadiaRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<EstadiaRepository> logger;

        public EstadiaRepository(HotelDbContext context, ILogger<EstadiaRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<EstadiaModel>>> GetAllAsync()
        {
            try
            {
                var estadias = await context.Estadias.AsNoTracking().ToListAsync();
                var models = estadias.Select(MapToModel).ToList();

                return new OperationResult<List<EstadiaModel>>
                {
                    Data = models,
                    Message = "Estadías consultadas correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar las estadías.");
                return Failure<List<EstadiaModel>>("No se pudieron consultar las estadías.");
            }
        }

        public async Task<OperationResult<EstadiaModel>> GetByIdAsync(int id)
        {
            try
            {
                var estadia = await context.Estadias.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                if (estadia is null)
                    return Failure<EstadiaModel>("La estadía no existe.");

                return new OperationResult<EstadiaModel>
                {
                    Data = MapToModel(estadia),
                    Message = "Estadía consultada correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar la estadía con id {Id}.", id);
                return Failure<EstadiaModel>("No se pudo consultar la estadía.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(EstadiaAdd estadia)
        {
            if (estadia is null)
                return Failure<bool>("El objeto EstadiaAdd es requerido.");

            var validation = ValidateEstadia(estadia);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new Estadia
                {
                    IdReserva = estadia.IdReserva,
                    IdUsuario = estadia.IdUsuario,
                    FechaEntrada = estadia.FechaEntrada,
                    FechaSalida = estadia.FechaSalida,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = estadia.CreatedUser
                };

                await context.Estadias.AddAsync(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Estadía registrada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar la estadía.");
                return Failure<bool>("No se pudo registrar la estadía. Verifique que la reserva y el usuario existan.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar la estadía.");
                return Failure<bool>("Ocurrió un error inesperado al registrar la estadía.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(EstadiaUpdate estadia)
        {
            if (estadia is null)
                return Failure<bool>("El objeto EstadiaUpdate es requerido.");

            var validation = ValidateEstadia(estadia);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.Estadias.FirstOrDefaultAsync(e => e.Id == estadia.Id);
                if (entity is null)
                    return Failure<bool>("La estadía no existe.");

                entity.IdReserva = estadia.IdReserva;
                entity.IdUsuario = estadia.IdUsuario;
                entity.FechaEntrada = estadia.FechaEntrada;
                entity.FechaSalida = estadia.FechaSalida;
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = estadia.ModifyUser;

                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Estadía actualizada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar la estadía.");
                return Failure<bool>("No se pudo actualizar la estadía. Verifique que la reserva y el usuario existan.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar la estadía.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar la estadía.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(EstadiaRemove estadia)
        {
            if (estadia is null)
                return Failure<bool>("El objeto EstadiaRemove es requerido.");

            try
            {
                var entity = await context.Estadias.FirstOrDefaultAsync(e => e.Id == estadia.Id);
                if (entity is null)
                    return Failure<bool>("La estadía no existe.");

                context.Estadias.Remove(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Estadía eliminada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar la estadía.");
                return Failure<bool>("No se pudo eliminar la estadía porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar la estadía.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar la estadía.");
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
