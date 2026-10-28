using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;
using ReservaHoteleraApp.DAL.Models.Result;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public class EstadiaRepository : IEstadiaRepository
    {
        private readonly HotelDbContext context;

        public EstadiaRepository(HotelDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<Estadia>>> GetAllAsync()
        {
            var estadias = await context.Estadias.AsNoTracking().ToListAsync();

            return new OperationResult<List<Estadia>>
            {
                Data = estadias,
                Message = "Estadías consultadas correctamente."
            };
        }

        public async Task<OperationResult<EstadiaModel>?> GetByIdAsync(int id)
        {
            var estadia = await context.Estadias.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (estadia is null)
                return null;

            return new OperationResult<EstadiaModel>
            {
                Data = MapToModel(estadia),
                Message = "Estadía consultada correctamente."
            };
        }

        public async Task<OperationResult<bool>> AddAsync(EstadiaAdd estadia)
        {
            if (estadia is null)
                return Failure<bool>("El objeto EstadiaAdd es requerido.");

            var validation = ValidateEstadia(estadia);
            if (validation is not null)
                return Failure<bool>(validation);

            var entity = new Estadia
            {
                IdReserva = estadia.IdReserva,
                IdUsuario = estadia.IdUsuario,
                FechaEntrada = estadia.FechaEntrada,
                FechaSalida = estadia.FechaSalida,
                CreatedDate = DateTime.UtcNow,
                CreatedUser = estadia.CreatedUser
            };

            await context.Estadias.AddAsync(entity);
            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Estadía registrada correctamente."
            };
        }

        public async Task<OperationResult<bool>> UpdateAsync(EstadiaUpdate estadia)
        {
            if (estadia is null)
                return Failure<bool>("El objeto EstadiaUpdate es requerido.");

            var validation = ValidateEstadia(estadia);
            if (validation is not null)
                return Failure<bool>(validation);

            var entity = await context.Estadias.FirstOrDefaultAsync(e => e.Id == estadia.Id);
            if (entity is null)
                return Failure<bool>("La estadía no existe.");

            entity.IdReserva = estadia.IdReserva;
            entity.IdUsuario = estadia.IdUsuario;
            entity.FechaEntrada = estadia.FechaEntrada;
            entity.FechaSalida = estadia.FechaSalida;
            entity.ModifyDate = DateTime.UtcNow;
            entity.ModifyUser = estadia.ModifyUser;

            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Estadía actualizada correctamente."
            };
        }

        public async Task<OperationResult<bool>> DeleteAsync(EstadiaRemove estadia)
        {
            if (estadia is null)
                return Failure<bool>("El objeto EstadiaRemove es requerido.");

            var entity = await context.Estadias.FirstOrDefaultAsync(e => e.Id == estadia.Id);
            if (entity is null)
                return Failure<bool>("La estadía no existe.");

            context.Estadias.Remove(entity);
            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Estadía eliminada correctamente."
            };
        }

        private static string? ValidateEstadia(EstadiaBase estadia)
        {
            if (estadia.IdReserva <= 0)
                return "El identificador de la reserva debe ser mayor que cero.";
            if (estadia.IdUsuario <= 0)
                return "El identificador del usuario debe ser mayor que cero.";
            if (estadia.FechaEntrada == default)
                return "La fecha de entrada es requerida.";
            if (estadia.FechaSalida.HasValue && estadia.FechaSalida.Value < estadia.FechaEntrada)
                return "La fecha de salida no puede ser anterior a la fecha de entrada.";

            return null;
        }

        private static EstadiaModel MapToModel(Estadia estadia)
        {
            return new EstadiaModel
            {
                Id = estadia.Id,
                IdReserva = estadia.IdReserva,
                IdUsuario = estadia.IdUsuario,
                FechaEntrada = estadia.FechaEntrada,
                FechaSalida = estadia.FechaSalida
            };
        }
    }
}
