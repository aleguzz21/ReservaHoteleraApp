using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    public class RegistroActividadServices : ServiceBase, IRegistroActividadServices
    {
        private readonly IRegistroActividadRepository _repository;

        public RegistroActividadServices(IRegistroActividadRepository repository, ILogger<RegistroActividadServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<RegistroActividadModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<RegistroActividadModel>>("Consultar registros de actividad", null, async () =>
            {
                return await _repository.GetAllAsync();
            });
        }

        public Task<OperationResult<RegistroActividadModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<RegistroActividadModel>("Consultar registro de actividad", null, async () =>
            {
                if (id <= 0)
                    return Failure<RegistroActividadModel>("El identificador del registro debe ser mayor que cero.");

                return await _repository.GetByIdAsync(id);
            });
        }

        public Task<OperationResult<bool>> AddAsync(RegistroActividadAdd? registroActividad)
        {
            return ExecuteAsync<bool>("Registrar actividad", registroActividad?.CreatedUser, async () =>
            {
                if (registroActividad is null)
                    return Failure<bool>("El objeto RegistroActividadAdd es requerido.");

                var validation = ValidateRegistroActividad(registroActividad);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(registroActividad);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Registro de actividad guardado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible guardar el registro de actividad.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(RegistroActividadUpdate? registroActividad)
        {
            return ExecuteAsync<bool>("Actualizar actividad", registroActividad?.ModifyUser, async () =>
            {
                if (registroActividad is null)
                    return Failure<bool>("El objeto RegistroActividadUpdate es requerido.");

                var validation = ValidateRegistroActividad(registroActividad);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(registroActividad);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Registro de actividad actualizado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar el registro de actividad.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(RegistroActividadRemove? registroActividad)
        {
            return DeleteEntityAsync(registroActividad, _repository.DeleteAsync, "el registro de actividad");
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
    }
}
