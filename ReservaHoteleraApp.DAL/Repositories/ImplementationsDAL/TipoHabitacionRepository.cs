using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class TipoHabitacionRepository : ITipoHabitacionRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<TipoHabitacionRepository> logger;

        public TipoHabitacionRepository(HotelDbContext context, ILogger<TipoHabitacionRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<TipoHabitacionModel>>> GetAllAsync()
        {
            try
            {
                var tipos = await context.TiposHabitacion.AsNoTracking().ToListAsync();
                var models = tipos.Select(MapToModel).ToList();

                return new OperationResult<List<TipoHabitacionModel>>
                {
                    Data = models,
                    Message = "Tipos de habitación consultados correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar los tipos de habitación.");
                return Failure<List<TipoHabitacionModel>>("No se pudieron consultar los tipos de habitación.");
            }
        }

        public async Task<OperationResult<TipoHabitacionModel>> GetByIdAsync(int id)
        {
            try
            {
                var tipo = await context.TiposHabitacion.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                if (tipo is null)
                    return Failure<TipoHabitacionModel>("El tipo de habitación no existe.");

                return new OperationResult<TipoHabitacionModel>
                {
                    Data = MapToModel(tipo),
                    Message = "Tipo de habitación consultado correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar el tipo de habitación con id {Id}.", id);
                return Failure<TipoHabitacionModel>("No se pudo consultar el tipo de habitación.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(TipoHabitacionAdd tipoHabitacion)
        {
            if (tipoHabitacion is null)
                return Failure<bool>("El objeto TipoHabitacionAdd es requerido.");

            var validation = ValidateTipoHabitacion(tipoHabitacion);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new TipoHabitacion
                {
                    Nombre = tipoHabitacion.Nombre!.Trim(),
                    Descripcion = tipoHabitacion.Descripcion?.Trim(),
                    Capacidad = tipoHabitacion.Capacidad,
                    Precio = tipoHabitacion.Precio,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = tipoHabitacion.CreatedUser
                };

                await context.TiposHabitacion.AddAsync(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Tipo de habitación registrado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar el tipo de habitación.");
                return Failure<bool>("No se pudo registrar el tipo de habitación. Verifique datos duplicados o referencias que no existen.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar el tipo de habitación.");
                return Failure<bool>("Ocurrió un error inesperado al registrar el tipo de habitación.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(TipoHabitacionUpdate tipoHabitacion)
        {
            if (tipoHabitacion is null)
                return Failure<bool>("El objeto TipoHabitacionUpdate es requerido.");

            var validation = ValidateTipoHabitacion(tipoHabitacion);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.TiposHabitacion.FirstOrDefaultAsync(t => t.Id == tipoHabitacion.Id);
                if (entity is null)
                    return Failure<bool>("El tipo de habitación no existe.");

                entity.Nombre = tipoHabitacion.Nombre!.Trim();
                entity.Descripcion = tipoHabitacion.Descripcion?.Trim();
                entity.Capacidad = tipoHabitacion.Capacidad;
                entity.Precio = tipoHabitacion.Precio;
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = tipoHabitacion.ModifyUser;

                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Tipo de habitación actualizado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar el tipo de habitación.");
                return Failure<bool>("No se pudo actualizar el tipo de habitación. Verifique datos duplicados o referencias que no existen.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar el tipo de habitación.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar el tipo de habitación.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(TipoHabitacionRemove tipoHabitacion)
        {
            if (tipoHabitacion is null)
                return Failure<bool>("El objeto TipoHabitacionRemove es requerido.");

            try
            {
                var entity = await context.TiposHabitacion.FirstOrDefaultAsync(t => t.Id == tipoHabitacion.Id);
                if (entity is null)
                    return Failure<bool>("El tipo de habitación no existe.");

                context.TiposHabitacion.Remove(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Tipo de habitación eliminado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar el tipo de habitación.");
                return Failure<bool>("No se pudo eliminar el tipo de habitación porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar el tipo de habitación.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar el tipo de habitación.");
            }
        }

        private static string? ValidateTipoHabitacion(TipoHabitacionBase tipoHabitacion)
        {
            if (string.IsNullOrWhiteSpace(tipoHabitacion.Nombre))
                return "El nombre del tipo de habitación es requerido.";
            if (tipoHabitacion.Nombre.Trim().Length > 100)
                return "El nombre del tipo de habitación no puede superar los 100 caracteres.";
            if (!string.IsNullOrWhiteSpace(tipoHabitacion.Descripcion) && tipoHabitacion.Descripcion.Trim().Length > 300)
                return "La descripción no puede superar los 300 caracteres.";
            if (tipoHabitacion.Capacidad <= 0)
                return "La capacidad debe ser mayor que cero.";
            if (tipoHabitacion.Precio < 0)
                return "El precio no puede ser negativo.";

            return null;
        }

        private static TipoHabitacionModel MapToModel(TipoHabitacion tipoHabitacion)
        {
            return new TipoHabitacionModel
            {
                Id = tipoHabitacion.Id,
                Nombre = tipoHabitacion.Nombre,
                Descripcion = tipoHabitacion.Descripcion,
                Capacidad = tipoHabitacion.Capacidad,
                Precio = tipoHabitacion.Precio
            };
        }
    }
}
