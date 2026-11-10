using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    // Repositorio del módulo Hotel: acceso real a la base de datos.
    // Recibe el DbContext (EF Core) y un logger para registrar los errores.
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelDbContext _context;
        private readonly ILogger<HotelRepository> _logger;

        public HotelRepository(HotelDbContext context, ILogger<HotelRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OperationResult<List<HotelModel>>> GetAllAsync()
        {
            try
            {
                // AsNoTracking = consulta de solo lectura (más rápida).
                var hoteles = await _context.Hoteles.AsNoTracking().ToListAsync();
                var models = hoteles.Select(MapToModel).ToList();
                return new OperationResult<List<HotelModel>>
                {
                    Data = models,
                    Message = "Hoteles consultados correctamente."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar los hoteles.");
                return Failure<List<HotelModel>>("No se pudieron consultar los hoteles.");
            }
        }

        public async Task<OperationResult<HotelModel>> GetByIdAsync(int id)
        {
            try
            {
                var hotel = await _context.Hoteles.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
                if (hotel is null)
                    return Failure<HotelModel>("El hotel no existe.");
                return new OperationResult<HotelModel>
                {
                    Data = MapToModel(hotel),
                    Message = "Hotel consultado correctamente."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar el hotel con id {Id}.", id);
                return Failure<HotelModel>("No se pudo consultar el hotel.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(HotelAdd hotelAdd)
        {
            try
            {
                var entity = new Hotel
                {
                    Nombre = hotelAdd.Nombre!.Trim(),
                    Direccion = hotelAdd.Direccion?.Trim(),
                    Telefono = hotelAdd.Telefono?.Trim(),
                    Ciudad = hotelAdd.Ciudad?.Trim(),
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = hotelAdd.CreatedUser
                };
                await _context.Hoteles.AddAsync(entity);
                await _context.SaveChangesAsync();
                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Hotel agregado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de la base de datos al registrar el hotel.");
                return Failure<bool>("No se pudo registrar el hotel. Verifique datos duplicados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el hotel.");
                return Failure<bool>("No se pudo registrar el hotel.");
            }
        }
        public async Task<OperationResult<bool>> UpdateAsync(HotelUpdate hotel)
        {
            if (hotel is null)
                return Failure<bool>("El objeto HotelUpdate es requerido.");

            var validation = ValidateHotel(hotel);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await _context.Hoteles.FirstOrDefaultAsync(h => h.Id == hotel.Id);
                if (entity is null)
                    return Failure<bool>("El hotel no existe.");

                entity.Nombre = hotel.Nombre!.Trim();
                entity.Direccion = hotel.Direccion?.Trim();
                entity.Telefono = hotel.Telefono?.Trim();
                entity.Ciudad = hotel.Ciudad?.Trim();
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = hotel.ModifyUser;

                await _context.SaveChangesAsync();
                return new OperationResult<bool> { Data = true, Message = "Hotel actualizado correctamente." };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al actualizar el hotel.");
                return Failure<bool>("No se pudo actualizar el hotel.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar el hotel.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar el hotel.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(HotelRemove hotel)
        {
            if (hotel is null)
                return Failure<bool>("El objeto HotelRemove es requerido.");

            try
            {
                var entity = await _context.Hoteles.FirstOrDefaultAsync(h => h.Id == hotel.Id);
                if (entity is null)
                    return Failure<bool>("El hotel no existe.");

                _context.Hoteles.Remove(entity);
                await _context.SaveChangesAsync();
                return new OperationResult<bool> { Data = true, Message = "Hotel eliminado correctamente." };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar el hotel.");
                return Failure<bool>("No se pudo eliminar el hotel porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar el hotel.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar el hotel.");
            }
        }

        // Reglas mínimas del hotel. Devuelve el mensaje de error, o null si todo está correcto.
        private static string? ValidateHotel(HotelBase hotel)
        {
            if (string.IsNullOrWhiteSpace(hotel.Nombre))
                return "El nombre del hotel es requerido.";
            if (hotel.Nombre.Trim().Length > 100)
                return "El nombre del hotel no puede superar los 100 caracteres.";
            if (!string.IsNullOrWhiteSpace(hotel.Direccion) && hotel.Direccion.Trim().Length > 200)
                return "La dirección no puede superar los 200 caracteres.";
            if (!string.IsNullOrWhiteSpace(hotel.Telefono) && hotel.Telefono.Trim().Length > 20)
                return "El teléfono no puede superar los 20 caracteres.";
            if (!string.IsNullOrWhiteSpace(hotel.Ciudad) && hotel.Ciudad.Trim().Length > 100)
                return "La ciudad no puede superar los 100 caracteres.";
            return null;
        }
        // Convierte la entidad (Hotel) en el modelo de la vista (HotelModel).
        private static HotelModel MapToModel(Hotel hotel)
        {
            return new HotelModel
            {
                Id = hotel.Id,
                Nombre = hotel.Nombre,
                Direccion = hotel.Direccion,
                Telefono = hotel.Telefono,
                Ciudad = hotel.Ciudad
            };
        }
    }
}
