using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.DAL.Models.Result;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public class TipoHabitacionRepository : ITipoHabitacionRepository
    {
        private readonly HotelDbContext context;

        public TipoHabitacionRepository(HotelDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<TipoHabitacion>>> GetAllAsync()
        {
            var tipos = await context.TiposHabitacion.AsNoTracking().ToListAsync();

            return new OperationResult<List<TipoHabitacion>>
            {
                Data = tipos,
                Message = "Tipos de habitación consultados correctamente."
            };
        }

        public async Task<OperationResult<TipoHabitacionModel>?> GetByIdAsync(int id)
        {
            var tipo = await context.TiposHabitacion.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (tipo is null)
                return null;

            return new OperationResult<TipoHabitacionModel>
            {
                Data = MapToModel(tipo),
                Message = "Tipo de habitación consultado correctamente."
            };
        }

        public async Task<OperationResult<bool>> AddAsync(TipoHabitacionAdd tipoHabitacion)
        {
            if (tipoHabitacion is null)
                return Failure<bool>("El objeto TipoHabitacionAdd es requerido.");

            var validation = ValidateTipoHabitacion(tipoHabitacion);
            if (validation is not null)
                return Failure<bool>(validation);

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

        public async Task<OperationResult<bool>> UpdateAsync(TipoHabitacionUpdate tipoHabitacion)
        {
            if (tipoHabitacion is null)
                return Failure<bool>("El objeto TipoHabitacionUpdate es requerido.");

            var validation = ValidateTipoHabitacion(tipoHabitacion);
            if (validation is not null)
                return Failure<bool>(validation);

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

        public async Task<OperationResult<bool>> DeleteAsync(TipoHabitacionRemove tipoHabitacion)
        {
            if (tipoHabitacion is null)
                return Failure<bool>("El objeto TipoHabitacionRemove es requerido.");

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
