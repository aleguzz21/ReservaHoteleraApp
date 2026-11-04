using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly HotelDbContext context;

        public ReservaRepository(HotelDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<Reserva>>> GetAllAsync()
        {
            var reservas = await context.Reservas.AsNoTracking().ToListAsync();

            return new OperationResult<List<Reserva>>
            {
                Data = reservas,
                Message = "Reservas consultadas correctamente."
            };
        }

        public async Task<OperationResult<ReservaModel>?> GetByIdAsync(int id)
        {
            var reserva = await context.Reservas.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (reserva is null)
                return null;

            return new OperationResult<ReservaModel>
            {
                Data = MapToModel(reserva),
                Message = "Reserva consultada correctamente."
            };
        }

        public async Task<OperationResult<bool>> AddAsync(ReservaAdd reserva)
        {
            if (reserva is null)
                return Failure<bool>("El objeto ReservaAdd es requerido.");

            var validation = ValidateReserva(reserva);
            if (validation is not null)
                return Failure<bool>(validation);

            var entity = new Reserva
            {
                IdCliente = reserva.IdCliente,
                IdHabitacion = reserva.IdHabitacion,
                CantidadPersonas = reserva.CantidadPersonas,
                Estado = string.IsNullOrWhiteSpace(reserva.Estado) ? "Pendiente" : reserva.Estado.Trim(),
                TotalReserva = reserva.TotalReserva,
                FechaEntrada = reserva.FechaEntrada,
                FechaSalida = reserva.FechaSalida,
                CreatedDate = DateTime.UtcNow,
                CreatedUser = reserva.CreatedUser
            };

            await context.Reservas.AddAsync(entity);
            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Reserva registrada correctamente."
            };
        }

        public async Task<OperationResult<bool>> UpdateAsync(ReservaUpdate reserva)
        {
            if (reserva is null)
                return Failure<bool>("El objeto ReservaUpdate es requerido.");

            var validation = ValidateReserva(reserva);
            if (validation is not null)
                return Failure<bool>(validation);

            var entity = await context.Reservas.FirstOrDefaultAsync(r => r.Id == reserva.Id);
            if (entity is null)
                return Failure<bool>("La reserva no existe.");

            entity.IdCliente = reserva.IdCliente;
            entity.IdHabitacion = reserva.IdHabitacion;
            entity.CantidadPersonas = reserva.CantidadPersonas;
            entity.Estado = string.IsNullOrWhiteSpace(reserva.Estado) ? "Pendiente" : reserva.Estado.Trim();
            entity.TotalReserva = reserva.TotalReserva;
            entity.FechaEntrada = reserva.FechaEntrada;
            entity.FechaSalida = reserva.FechaSalida;
            entity.ModifyDate = DateTime.UtcNow;
            entity.ModifyUser = reserva.ModifyUser;

            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Reserva actualizada correctamente."
            };
        }

        public async Task<OperationResult<bool>> DeleteAsync(ReservaRemove reserva)
        {
            if (reserva is null)
                return Failure<bool>("El objeto ReservaRemove es requerido.");

            var entity = await context.Reservas.FirstOrDefaultAsync(r => r.Id == reserva.Id);
            if (entity is null)
                return Failure<bool>("La reserva no existe.");

            context.Reservas.Remove(entity);
            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Reserva eliminada correctamente."
            };
        }

        private static string? ValidateReserva(ReservaBase reserva)
        {
            if (reserva.IdCliente <= 0)
                return "El identificador del cliente debe ser mayor que cero.";
            if (reserva.IdHabitacion <= 0)
                return "El identificador de la habitación debe ser mayor que cero.";
            if (reserva.CantidadPersonas <= 0)
                return "La cantidad de personas debe ser mayor que cero.";
            if (!string.IsNullOrWhiteSpace(reserva.Estado) && reserva.Estado.Trim().Length > 20)
                return "El estado no puede superar los 20 caracteres.";
            if (reserva.TotalReserva < 0)
                return "El total de la reserva no puede ser negativo.";
            if (reserva.FechaEntrada == default)
                return "La fecha de entrada es requerida.";
            if (reserva.FechaSalida.HasValue && reserva.FechaSalida.Value < reserva.FechaEntrada)
                return "La fecha de salida no puede ser anterior a la fecha de entrada.";

            return null;
        }

        private static ReservaModel MapToModel(Reserva reserva)
        {
            return new ReservaModel
            {
                Id = reserva.Id,
                IdCliente = reserva.IdCliente,
                IdHabitacion = reserva.IdHabitacion,
                CantidadPersonas = reserva.CantidadPersonas,
                Estado = reserva.Estado,
                TotalReserva = reserva.TotalReserva,
                FechaEntrada = reserva.FechaEntrada,
                FechaSalida = reserva.FechaSalida
            };
        }
    }
}
