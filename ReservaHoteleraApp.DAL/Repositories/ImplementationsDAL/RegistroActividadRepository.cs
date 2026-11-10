using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class RegistroActividadRepository : IRegistroActividadRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<RegistroActividadRepository> logger;

        public RegistroActividadRepository(HotelDbContext context, ILogger<RegistroActividadRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<RegistroActividadModel>>> GetAllAsync()
        {
            try
            {
                var registros = await context.RegistrosActividad.AsNoTracking().ToListAsync();
                var models = registros.Select(MapToModel).ToList();

                return new OperationResult<List<RegistroActividadModel>>
                {
                    Data = models,
                    Message = "Registros de actividad consultados correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar los registros de actividad.");
                return Failure<List<RegistroActividadModel>>("No se pudieron consultar los registros de actividad.");
            }
        }

        public async Task<OperationResult<RegistroActividadModel>> GetByIdAsync(int id)
        {
            try
            {
                var registro = await context.RegistrosActividad.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
                if (registro is null)
                    return Failure<RegistroActividadModel>("El registro de actividad no existe.");

                return new OperationResult<RegistroActividadModel>
                {
                    Data = MapToModel(registro),
                    Message = "Registro de actividad consultado correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar el registro de actividad con id {Id}.", id);
                return Failure<RegistroActividadModel>("No se pudo consultar el registro de actividad.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(RegistroActividadAdd registroActividad)
        {
            if (registroActividad is null)
                return Failure<bool>("El objeto RegistroActividadAdd es requerido.");

            var validation = ValidateRegistroActividad(registroActividad);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new RegistroActividad
                {
                    IdUsuario = registroActividad.IdUsuario,
                    Accion = registroActividad.Accion!.Trim(),
                    FechaHora = registroActividad.FechaHora == default ? DateTime.UtcNow : registroActividad.FechaHora,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = registroActividad.CreatedUser
                };

                await context.RegistrosActividad.AddAsync(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Registro de actividad guardado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al guardar el registro de actividad.");
                return Failure<bool>("No se pudo guardar el registro de actividad. Verifique que el usuario exista.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al guardar el registro de actividad.");
                return Failure<bool>("Ocurrió un error inesperado al guardar el registro de actividad.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(RegistroActividadUpdate registroActividad)
        {
            if (registroActividad is null)
                return Failure<bool>("El objeto RegistroActividadUpdate es requerido.");

            var validation = ValidateRegistroActividad(registroActividad);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.RegistrosActividad.FirstOrDefaultAsync(r => r.Id == registroActividad.Id);
                if (entity is null)
                    return Failure<bool>("El registro de actividad no existe.");

                entity.IdUsuario = registroActividad.IdUsuario;
                entity.Accion = registroActividad.Accion!.Trim();
                entity.FechaHora = registroActividad.FechaHora == default ? DateTime.UtcNow : registroActividad.FechaHora;
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = registroActividad.ModifyUser;

                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Registro de actividad actualizado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar el registro de actividad.");
                return Failure<bool>("No se pudo actualizar el registro de actividad. Verifique que el usuario exista.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar el registro de actividad.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar el registro de actividad.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(RegistroActividadRemove registroActividad)
        {
            if (registroActividad is null)
                return Failure<bool>("El objeto RegistroActividadRemove es requerido.");

            try
            {
                var entity = await context.RegistrosActividad.FirstOrDefaultAsync(r => r.Id == registroActividad.Id);
                if (entity is null)
                    return Failure<bool>("El registro de actividad no existe.");

                context.RegistrosActividad.Remove(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Registro de actividad eliminado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar el registro de actividad.");
                return Failure<bool>("No se pudo eliminar el registro de actividad porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar el registro de actividad.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar el registro de actividad.");
            }
        }

        private static string? ValidateRegistroActividad(RegistroActividadBase registroActividad)
        {
            if (registroActividad.IdUsuario <= 0)
                return "El identificador del usuario debe ser mayor que cero.";
            if (string.IsNullOrWhiteSpace(registroActividad.Accion))
                return "La acción es requerida.";
            if (registroActividad.Accion.Trim().Length > 255)
                return "La acción no puede superar los 255 caracteres.";

            return null;
        }

        private static RegistroActividadModel MapToModel(RegistroActividad registroActividad)
        {
            return new RegistroActividadModel
            {
                Id = registroActividad.Id,
                IdUsuario = registroActividad.IdUsuario,
                Accion = registroActividad.Accion,
                FechaHora = registroActividad.FechaHora
            };
        }
    }
}
