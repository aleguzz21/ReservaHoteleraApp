using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Piso;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    // Repositorio del módulo Piso: aquí vive el acceso real a la base de datos.
    // Recibe el DbContext (EF Core) y un logger para registrar los errores.
    public class PisoRepository : IPisoRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<PisoRepository> logger;

        public PisoRepository(HotelDbContext context, ILogger<PisoRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<PisoModel>>> GetAllAsync()
        {
            try
            {
                // AsNoTracking = consulta de solo lectura (más rápida).
                // Include(p => p.Bloque) trae también el bloque relacionado (para mostrar su nombre).
                var pisos = await context.Pisos.AsNoTracking().Include(p => p.Bloque).ToListAsync();
                var models = pisos.Select(MapToModel).ToList();
                return new OperationResult<List<PisoModel>>
                {
                    Data = models,
                    Message = "Pisos consultados correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar los pisos.");
                return Failure<List<PisoModel>>("No se pudieron consultar los pisos.");
            }
        }

        public async Task<OperationResult<PisoModel>> GetByIdAsync(int id)
        {
            try
            {
                var piso = await context.Pisos.AsNoTracking().Include(p => p.Bloque).FirstOrDefaultAsync(p => p.Id == id);
                if (piso is null)
                    return Failure<PisoModel>("El piso no existe.");

                return new OperationResult<PisoModel>
                {
                    Data = MapToModel(piso),
                    Message = "Piso consultado correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar el piso con id {Id}.", id);
                return Failure<PisoModel>("No se pudo consultar el piso.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(PisoAdd piso)
        {
            if (piso is null)
                return Failure<bool>("El objeto PisoAdd es requerido.");

            var validation = ValidatePiso(piso);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new Piso
                {
                    Numero = piso.Numero,
                    IdBloque = piso.IdBloque,
                    Descripcion = piso.Descripcion?.Trim(),
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = piso.CreatedUser
                };
                await context.Pisos.AddAsync(entity);
                await context.SaveChangesAsync();
                return new OperationResult<bool> { Data = true, Message = "Piso registrado correctamente." };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar el piso.");
                return Failure<bool>("No se pudo registrar el piso. Verifique que el bloque exista.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar el piso.");
                return Failure<bool>("Ocurrió un error inesperado al registrar el piso.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(PisoUpdate piso)
        {
            if (piso is null)
                return Failure<bool>("El objeto PisoUpdate es requerido.");

            var validation = ValidatePiso(piso);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.Pisos.FirstOrDefaultAsync(p => p.Id == piso.Id);
                if (entity is null)
                    return Failure<bool>("El piso no existe.");

                entity.Numero = piso.Numero;
                entity.IdBloque = piso.IdBloque;
                entity.Descripcion = piso.Descripcion?.Trim();
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = piso.ModifyUser;

                await context.SaveChangesAsync();
                return new OperationResult<bool> { Data = true, Message = "Piso actualizado correctamente." };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar el piso.");
                return Failure<bool>("No se pudo actualizar el piso. Verifique que el bloque exista.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar el piso.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar el piso.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(PisoRemove piso)
        {
            if (piso is null)
                return Failure<bool>("El objeto PisoRemove es requerido.");

            try
            {
                var entity = await context.Pisos.FirstOrDefaultAsync(p => p.Id == piso.Id);
                if (entity is null)
                    return Failure<bool>("El piso no existe.");

                context.Pisos.Remove(entity);
                await context.SaveChangesAsync();
                return new OperationResult<bool> { Data = true, Message = "Piso eliminado correctamente." };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar el piso.");
                return Failure<bool>("No se pudo eliminar el piso porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar el piso.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar el piso.");
            }
        }

        // Reglas mínimas del piso. Devuelve el mensaje de error, o null si todo está correcto.
        private static string? ValidatePiso(PisoBase piso)
        {
            if (piso.Numero <= 0)
                return "El número del piso debe ser mayor que cero.";
            if (piso.IdBloque <= 0)
                return "Debe seleccionar un bloque.";
            if (!string.IsNullOrWhiteSpace(piso.Descripcion) && piso.Descripcion.Trim().Length > 300)
                return "La descripción no puede superar los 300 caracteres.";
            return null;
        }

        // Convierte la entidad de base de datos (Piso) en el modelo que usa la vista (PisoModel).
        private static PisoModel MapToModel(Piso piso)
        {
            return new PisoModel
            {
                Id = piso.Id,
                Numero = piso.Numero,
                IdBloque = piso.IdBloque,
                Descripcion = piso.Descripcion,
                BloqueNombre = piso.Bloque?.Nombre
            };
        }
    }
}
