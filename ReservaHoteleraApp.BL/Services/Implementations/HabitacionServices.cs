using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    public class HabitacionServices : ServiceBase, IHabitacionServices
    {
        private readonly IHabitacionRepository _repository;

        public HabitacionServices(IHabitacionRepository repository, ILogger<HabitacionServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<HabitacionModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<HabitacionModel>>("Consultar habitaciones", null, async () =>
            {
                var result = await _repository.GetAllAsync();
                if (!result.Success || result.Data is null)
                    return Failure<List<HabitacionModel>>(result.Message ?? "No fue posible consultar las habitaciones.");

                var models = result.Data.Select(MapToModel).ToList();
                return new OperationResult<List<HabitacionModel>>
                {
                    Data = models,
                    Message = "Habitaciones consultadas correctamente."
                };
            });
        }

        public Task<OperationResult<HabitacionModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<HabitacionModel>("Consultar habitación", null, async () =>
            {
                if (id <= 0)
                    return Failure<HabitacionModel>("El identificador de la habitación debe ser mayor que cero.");

                var result = await _repository.GetByIdAsync(id);
                if (result is null || !result.Success || result.Data is null)
                    return Failure<HabitacionModel>("La habitación no existe.");

                return result;
            });
        }

        public Task<OperationResult<bool>> AddAsync(HabitacionAdd? habitacion)
        {
            return ExecuteAsync<bool>("Registrar habitación", habitacion?.CreatedUser, async () =>
            {
                if (habitacion is null)
                    return Failure<bool>("El objeto HabitacionAdd es requerido.");

                var validation = ValidateHabitacion(habitacion);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(habitacion);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Habitación registrada correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar la habitación.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate? habitacion)
        {
            return ExecuteAsync<bool>("Actualizar habitación", habitacion?.ModifyUser, async () =>
            {
                if (habitacion is null)
                    return Failure<bool>("El objeto HabitacionUpdate es requerido.");

                var validation = ValidateHabitacion(habitacion);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(habitacion);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Habitación actualizada correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar la habitación.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(HabitacionRemove? habitacion)
        {
            return DeleteEntityAsync(habitacion, _repository.DeleteAsync, "la habitación");
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
