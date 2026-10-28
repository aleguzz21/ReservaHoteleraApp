using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;
using ReservaHoteleraApp.DAL.Models.Result;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public class RegistroActividadRepository : IRegistroActividadRepository
    {
        private readonly HotelDbContext context;

        public RegistroActividadRepository(HotelDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<RegistroActividad>>> GetAllAsync()
        {
            var registros = await context.RegistrosActividad.AsNoTracking().ToListAsync();

            return new OperationResult<List<RegistroActividad>>
            {
                Data = registros,
                Message = "Registros de actividad consultados correctamente."
            };
        }

        public async Task<OperationResult<RegistroActividadModel>?> GetByIdAsync(int id)
        {
            var registro = await context.RegistrosActividad.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (registro is null)
                return null;

            return new OperationResult<RegistroActividadModel>
            {
                Data = MapToModel(registro),
                Message = "Registro de actividad consultado correctamente."
            };
        }

        public async Task<OperationResult<bool>> AddAsync(RegistroActividadAdd registroActividad)
        {
            if (registroActividad is null)
                return Failure<bool>("El objeto RegistroActividadAdd es requerido.");

            var validation = ValidateRegistroActividad(registroActividad);
            if (validation is not null)
                return Failure<bool>(validation);

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

        public async Task<OperationResult<bool>> UpdateAsync(RegistroActividadUpdate registroActividad)
        {
            if (registroActividad is null)
                return Failure<bool>("El objeto RegistroActividadUpdate es requerido.");

            var validation = ValidateRegistroActividad(registroActividad);
            if (validation is not null)
                return Failure<bool>(validation);

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

        public async Task<OperationResult<bool>> DeleteAsync(RegistroActividadRemove registroActividad)
        {
            if (registroActividad is null)
                return Failure<bool>("El objeto RegistroActividadRemove es requerido.");

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
