using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Pago;
using ReservaHoteleraApp.DAL.Models.Result;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly HotelDbContext context;

        public PagoRepository(HotelDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<Pago>>> GetAllAsync()
        {
            var pagos = await context.Pagos.AsNoTracking().ToListAsync();

            return new OperationResult<List<Pago>>
            {
                Data = pagos,
                Message = "Pagos consultados correctamente."
            };
        }

        public async Task<OperationResult<PagoModel>?> GetByIdAsync(int id)
        {
            var pago = await context.Pagos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (pago is null)
                return null;

            return new OperationResult<PagoModel>
            {
                Data = MapToModel(pago),
                Message = "Pago consultado correctamente."
            };
        }

        public async Task<OperationResult<bool>> AddAsync(PagoAdd pago)
        {
            if (pago is null)
                return Failure<bool>("El objeto PagoAdd es requerido.");

            var validation = ValidatePago(pago);
            if (validation is not null)
                return Failure<bool>(validation);

            var entity = new Pago
            {
                IdReserva = pago.IdReserva,
                Monto = pago.Monto,
                Fecha = pago.Fecha == default ? DateTime.UtcNow : pago.Fecha,
                MetodoPago = pago.MetodoPago!.Trim(),
                CreatedDate = DateTime.UtcNow,
                CreatedUser = pago.CreatedUser
            };

            await context.Pagos.AddAsync(entity);
            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Pago registrado correctamente."
            };
        }

        public async Task<OperationResult<bool>> UpdateAsync(PagoUpdate pago)
        {
            if (pago is null)
                return Failure<bool>("El objeto PagoUpdate es requerido.");

            var validation = ValidatePago(pago);
            if (validation is not null)
                return Failure<bool>(validation);

            var entity = await context.Pagos.FirstOrDefaultAsync(p => p.Id == pago.Id);
            if (entity is null)
                return Failure<bool>("El pago no existe.");

            entity.IdReserva = pago.IdReserva;
            entity.Monto = pago.Monto;
            entity.Fecha = pago.Fecha == default ? DateTime.UtcNow : pago.Fecha;
            entity.MetodoPago = pago.MetodoPago!.Trim();
            entity.ModifyDate = DateTime.UtcNow;
            entity.ModifyUser = pago.ModifyUser;

            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Pago actualizado correctamente."
            };
        }

        public async Task<OperationResult<bool>> DeleteAsync(PagoRemove pago)
        {
            if (pago is null)
                return Failure<bool>("El objeto PagoRemove es requerido.");

            var entity = await context.Pagos.FirstOrDefaultAsync(p => p.Id == pago.Id);
            if (entity is null)
                return Failure<bool>("El pago no existe.");

            context.Pagos.Remove(entity);
            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Pago eliminado correctamente."
            };
        }

        private static string? ValidatePago(PagoBase pago)
        {
            if (pago.IdReserva <= 0)
                return "El identificador de la reserva debe ser mayor que cero.";
            if (pago.Monto <= 0)
                return "El monto debe ser mayor que cero.";
            if (string.IsNullOrWhiteSpace(pago.MetodoPago))
                return "El método de pago es requerido.";
            if (pago.MetodoPago.Trim().Length > 50)
                return "El método de pago no puede superar los 50 caracteres.";

            return null;
        }

        private static PagoModel MapToModel(Pago pago)
        {
            return new PagoModel
            {
                Id = pago.Id,
                IdReserva = pago.IdReserva,
                Monto = pago.Monto,
                Fecha = pago.Fecha,
                MetodoPago = pago.MetodoPago
            };
        }
    }
}
