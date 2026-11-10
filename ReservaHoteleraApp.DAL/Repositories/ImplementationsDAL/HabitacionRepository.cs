using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class HabitacionRepository : IHabitacionRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<HabitacionRepository> logger;

        public HabitacionRepository(HotelDbContext context, ILogger<HabitacionRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<HabitacionModel>>> GetAllAsync()
        {
            try
            {
                var habitaciones = await context.Habitaciones.AsNoTracking().ToListAsync();
                var models = habitaciones.Select(MapToModel).ToList();

                return new OperationResult<List<HabitacionModel>>
                {
                    Data = models,
                    Message = "Habitaciones consultadas correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar las habitaciones.");
                return Failure<List<HabitacionModel>>("No se pudieron consultar las habitaciones.");
            }
        }

        public async Task<OperationResult<HabitacionModel>> GetByIdAsync(int id)
        {
            try
            {
                var habitacion = await context.Habitaciones.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
                if (habitacion is null)
                    return Failure<HabitacionModel>("La habitación no existe.");

                return new OperationResult<HabitacionModel>
                {
                    Data = MapToModel(habitacion),
                    Message = "Habitación consultada correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar la habitación con id {Id}.", id);
                return Failure<HabitacionModel>("No se pudo consultar la habitación.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(HabitacionAdd habitacion)
        {
            if (habitacion is null)
                return Failure<bool>("El objeto HabitacionAdd es requerido.");

            var validation = ValidateHabitacion(habitacion);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new Habitacion
                {
                    Numero = habitacion.Numero!.Trim(),
                    IdTipoHabitacion = habitacion.IdTipoHabitacion,
                    PrecioNoche = habitacion.PrecioNoche,
                    Estado = string.IsNullOrWhiteSpace(habitacion.Estado) ? "Disponible" : habitacion.Estado.Trim(),
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = habitacion.CreatedUser
                };

                await context.Habitaciones.AddAsync(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Habitación registrada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar la habitación.");
                return Failure<bool>("No se pudo registrar la habitación. Verifique datos duplicados o referencias que no existen.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar la habitación.");
                return Failure<bool>("Ocurrió un error inesperado al registrar la habitación.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate habitacion)
        {
            if (habitacion is null)
                return Failure<bool>("El objeto HabitacionUpdate es requerido.");

            var validation = ValidateHabitacion(habitacion);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.Habitaciones.FirstOrDefaultAsync(h => h.Id == habitacion.Id);
                if (entity is null)
                    return Failure<bool>("La habitación no existe.");

                entity.Numero = habitacion.Numero!.Trim();
                entity.IdTipoHabitacion = habitacion.IdTipoHabitacion;
                entity.PrecioNoche = habitacion.PrecioNoche;
                entity.Estado = string.IsNullOrWhiteSpace(habitacion.Estado) ? "Disponible" : habitacion.Estado.Trim();
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = habitacion.ModifyUser;

                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Habitación actualizada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar la habitación.");
                return Failure<bool>("No se pudo actualizar la habitación. Verifique datos duplicados o referencias que no existen.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar la habitación.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar la habitación.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(HabitacionRemove habitacion)
        {
            if (habitacion is null)
                return Failure<bool>("El objeto HabitacionRemove es requerido.");

            try
            {
                var entity = await context.Habitaciones.FirstOrDefaultAsync(h => h.Id == habitacion.Id);
                if (entity is null)
                    return Failure<bool>("La habitación no existe.");

                context.Habitaciones.Remove(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Habitación eliminada correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar la habitación.");
                return Failure<bool>("No se pudo eliminar la habitación porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar la habitación.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar la habitación.");
            }
        }

        private static string? ValidateHabitacion(HabitacionBase habitacion)
        {
            if (string.IsNullOrWhiteSpace(habitacion.Numero))
                return "El número de habitación es requerido.";
            if (habitacion.Numero.Trim().Length > 10)
                return "El número de habitación no puede superar los 10 caracteres.";
            if (habitacion.IdTipoHabitacion <= 0)
                return "El identificador del tipo de habitación debe ser mayor que cero.";
            if (habitacion.PrecioNoche < 0)
                return "El precio por noche no puede ser negativo.";
            if (!string.IsNullOrWhiteSpace(habitacion.Estado) && habitacion.Estado.Trim().Length > 20)
                return "El estado no puede superar los 20 caracteres.";

            return null;
        }

        private static HabitacionModel MapToModel(Habitacion habitacion)
        {
            return new HabitacionModel
            {
                Id = habitacion.Id,
                Numero = habitacion.Numero,
                IdTipoHabitacion = habitacion.IdTipoHabitacion,
                PrecioNoche = habitacion.PrecioNoche,
                Estado = habitacion.Estado
            };
        }
    }
}
