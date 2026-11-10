using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    // Servicio de negocio del módulo Tipo de Habitación.
    // Valida los datos y luego delega el trabajo al repositorio (ITipoHabitacionRepository).
    public class TipoHabitacionServices : ServiceBase, ITipoHabitacionServices
    {
        private readonly ITipoHabitacionRepository _repository;

        public TipoHabitacionServices(ITipoHabitacionRepository repository, ILogger<TipoHabitacionServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<TipoHabitacionModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<TipoHabitacionModel>>("Consultar tipos de habitación", null, async () =>
            {
                return await _repository.GetAllAsync();
            });
        }

        public Task<OperationResult<TipoHabitacionModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<TipoHabitacionModel>("Consultar tipo de habitación", null, async () =>
            {
                if (id <= 0)
                    return Failure<TipoHabitacionModel>("El identificador debe ser mayor que cero.");

                return await _repository.GetByIdAsync(id);
            });
        }

        public Task<OperationResult<bool>> AddAsync(TipoHabitacionAdd? tipoHabitacion)
        {
            return ExecuteAsync<bool>("Registrar tipo de habitación", tipoHabitacion?.CreatedUser, async () =>
            {
                if (tipoHabitacion is null)
                    return Failure<bool>("El objeto TipoHabitacionAdd es requerido.");

                var validation = ValidateTipoHabitacion(tipoHabitacion);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(tipoHabitacion);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Tipo de habitación registrado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar el tipo de habitación.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(TipoHabitacionUpdate? tipoHabitacion)
        {
            return ExecuteAsync<bool>("Actualizar tipo de habitación", tipoHabitacion?.ModifyUser, async () =>
            {
                if (tipoHabitacion is null)
                    return Failure<bool>("El objeto TipoHabitacionUpdate es requerido.");

                var validation = ValidateTipoHabitacion(tipoHabitacion);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(tipoHabitacion);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Tipo de habitación actualizado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar el tipo de habitación.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(TipoHabitacionRemove? tipoHabitacion)
        {
            return DeleteEntityAsync(tipoHabitacion, _repository.DeleteAsync, "el tipo de habitación");
        }

        // Reglas de negocio del tipo de habitación. Devuelve el mensaje de error, o null si todo está correcto.
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
    }
}
