using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services
{
    public class RegistroActividadServices : IRegistroActividadServices
    {
        private readonly IRegistroActividadRepository _repository;
        private readonly ILogger<RegistroActividadServices> _logger;

        public RegistroActividadServices(IRegistroActividadRepository repository, ILogger<RegistroActividadServices> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public Task<OperationResult<List<RegistroActividadModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<RegistroActividadModel>>("Consultar registros de actividad", null, async () =>
            {
                var result = await _repository.GetAllAsync();
                if (!result.Success || result.Data is null)
                    return Failure<List<RegistroActividadModel>>(result.Message ?? "No fue posible consultar los registros de actividad.");

                var models = result.Data.Select(MapToModel).ToList();
                return new OperationResult<List<RegistroActividadModel>>
                {
                    Data = models,
                    Message = "Registros de actividad consultados correctamente."
                };
            });
        }

        public Task<OperationResult<RegistroActividadModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<RegistroActividadModel>("Consultar registro de actividad", null, async () =>
            {
                if (id <= 0)
                    return Failure<RegistroActividadModel>("El identificador del registro debe ser mayor que cero.");

                var result = await _repository.GetByIdAsync(id);
                if (result is null || !result.Success || result.Data is null)
                    return Failure<RegistroActividadModel>("El registro de actividad no existe.");

                return result;
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

        private async Task<OperationResult<T>> ExecuteAsync<T>(string operation, int? userId, Func<Task<OperationResult<T>>> action)
        {
            try
            {
                _logger.LogInformation("Ejecutando {Operation}. Usuario: {UserId}", operation, userId);
                return await action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar {Operation}. Usuario: {UserId}", operation, userId);
                return Failure<T>("Ocurrió un error inesperado. Intente nuevamente.");
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
