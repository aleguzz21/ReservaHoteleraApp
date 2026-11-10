using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Bloque;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class BloqueRepository : IBloqueRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<BloqueRepository> logger;

        public BloqueRepository(HotelDbContext context, ILogger<BloqueRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<BloqueModel>>> GetAllAsync()
        {
            try
            {
                var bloques = await context.Bloques.AsNoTracking().Include(b => b.Hotel).ToListAsync();
                var models = bloques.Select(MapToModel).ToList();
                return new OperationResult<List<BloqueModel>>
                {
                    Data = models,
                    Message = "Bloques consultados correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar los bloques.");
                return Failure<List<BloqueModel>>("No se pudieron consultar los bloques.");
            }
        }

        public async Task<OperationResult<BloqueModel>> GetByIdAsync(int id)
        {
            try
            {
                var bloque = await context.Bloques.AsNoTracking().Include(b => b.Hotel).FirstOrDefaultAsync(b => b.Id == id);
                if (bloque is null)
                    return Failure<BloqueModel>("El bloque no existe.");

                return new OperationResult<BloqueModel>
                {
                    Data = MapToModel(bloque),
                    Message = "Bloque consultado correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar el bloque con id {Id}.", id);
                return Failure<BloqueModel>("No se pudo consultar el bloque.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(BloqueAdd bloque)
        {
            if (bloque is null)
                return Failure<bool>("El objeto BloqueAdd es requerido.");

            var validation = ValidateBloque(bloque);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new Bloque
                {
                    Nombre = bloque.Nombre!.Trim(),
                    IdHotel = bloque.IdHotel,
                    Descripcion = bloque.Descripcion?.Trim(),
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = bloque.CreatedUser
                };
                await context.Bloques.AddAsync(entity);
                await context.SaveChangesAsync();
                return new OperationResult<bool> { Data = true, Message = "Bloque registrado correctamente." };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar el bloque.");
                return Failure<bool>("No se pudo registrar el bloque. Verifique que el hotel exista.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar el bloque.");
                return Failure<bool>("Ocurrió un error inesperado al registrar el bloque.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(BloqueUpdate bloque)
        {
            if (bloque is null)
                return Failure<bool>("El objeto BloqueUpdate es requerido.");

            var validation = ValidateBloque(bloque);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.Bloques.FirstOrDefaultAsync(b => b.Id == bloque.Id);
                if (entity is null)
                    return Failure<bool>("El bloque no existe.");

                entity.Nombre = bloque.Nombre!.Trim();
                entity.IdHotel = bloque.IdHotel;
                entity.Descripcion = bloque.Descripcion?.Trim();
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = bloque.ModifyUser;

                await context.SaveChangesAsync();
                return new OperationResult<bool> { Data = true, Message = "Bloque actualizado correctamente." };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar el bloque.");
                return Failure<bool>("No se pudo actualizar el bloque. Verifique que el hotel exista.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar el bloque.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar el bloque.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(BloqueRemove bloque)
        {
            if (bloque is null)
                return Failure<bool>("El objeto BloqueRemove es requerido.");

            try
            {
                var entity = await context.Bloques.FirstOrDefaultAsync(b => b.Id == bloque.Id);
                if (entity is null)
                    return Failure<bool>("El bloque no existe.");

                context.Bloques.Remove(entity);
                await context.SaveChangesAsync();
                return new OperationResult<bool> { Data = true, Message = "Bloque eliminado correctamente." };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar el bloque.");
                return Failure<bool>("No se pudo eliminar el bloque porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar el bloque.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar el bloque.");
            }
        }

        private static string? ValidateBloque(BloqueBase bloque)
        {
            if (string.IsNullOrWhiteSpace(bloque.Nombre))
                return "El nombre del bloque es requerido.";
            if (bloque.Nombre.Trim().Length > 100)
                return "El nombre del bloque no puede superar los 100 caracteres.";
            if (bloque.IdHotel <= 0)
                return "Debe seleccionar un hotel.";
            if (!string.IsNullOrWhiteSpace(bloque.Descripcion) && bloque.Descripcion.Trim().Length > 300)
                return "La descripción no puede superar los 300 caracteres.";
            return null;
        }

        private static BloqueModel MapToModel(Bloque bloque)
        {
            return new BloqueModel
            {
                Id = bloque.Id,
                Nombre = bloque.Nombre,
                IdHotel = bloque.IdHotel,
                Descripcion = bloque.Descripcion,
                HotelNombre = bloque.Hotel?.Nombre
            };
        }
    }
}
