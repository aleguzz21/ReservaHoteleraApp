using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
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
                var result = await _repository.GetAllAsync();
                if (!result.Success || result.Data is null)
                    return Failure<List<TipoHabitacionModel>>(result.Message ?? "No fue posible consultar los tipos de habitación.");

                var models = result.Data.Select(MapToModel).ToList();
                return new OperationResult<List<TipoHabitacionModel>>
                {
                    Data = models,
                    Message = "Tipos de habitación consultados correctamente."
                };
            });
        }

        public Task<OperationResult<TipoHabitacionModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<TipoHabitacionModel>("Consultar tipo de habitación", null, async () =>
            {
                if (id <= 0)
                    return Failure<TipoHabitacionModel>("El identificador debe ser mayor que cero.");

                var result = await _repository.GetByIdAsync(id);
                if (result is null || !result.Success || result.Data is null)
                    return Failure<TipoHabitacionModel>("El tipo de habitación no existe.");

                return result;
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
