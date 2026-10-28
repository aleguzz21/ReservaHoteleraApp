using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Result;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public class HabitacionRepository : IHabitacionRepository
    {
        private readonly HotelDbContext context;

        public HabitacionRepository(HotelDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<Habitacion>>> GetAllAsync()
        {
            var habitaciones = await context.Habitaciones.AsNoTracking().ToListAsync();

            return new OperationResult<List<Habitacion>>
            {
                Data = habitaciones,
                Message = "Habitaciones consultadas correctamente."
            };
        }

        public async Task<OperationResult<HabitacionModel>?> GetByIdAsync(int id)
        {
            var habitacion = await context.Habitaciones.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
            if (habitacion is null)
                return null;

            return new OperationResult<HabitacionModel>
            {
                Data = MapToModel(habitacion),
                Message = "Habitación consultada correctamente."
            };
        }

        public async Task<OperationResult<bool>> AddAsync(HabitacionAdd habitacion)
        {
            if (habitacion is null)
                return Failure<bool>("El objeto HabitacionAdd es requerido.");

            var validation = ValidateHabitacion(habitacion);
            if (validation is not null)
                return Failure<bool>(validation);

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

        public async Task<OperationResult<bool>> UpdateAsync(HabitacionUpdate habitacion)
        {
            if (habitacion is null)
                return Failure<bool>("El objeto HabitacionUpdate es requerido.");

            var validation = ValidateHabitacion(habitacion);
            if (validation is not null)
                return Failure<bool>(validation);

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

        public async Task<OperationResult<bool>> DeleteAsync(HabitacionRemove habitacion)
        {
            if (habitacion is null)
                return Failure<bool>("El objeto HabitacionRemove es requerido.");

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
