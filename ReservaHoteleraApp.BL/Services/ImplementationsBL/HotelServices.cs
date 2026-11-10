using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.BL.Services.Base;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using System;
using System.Collections.Generic;
using System.Text;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services.Implementations
{
    // Servicio de negocio del módulo Hotel.
    // Valida los datos y luego delega el trabajo al repositorio (IHotelRepository).
    public class HotelServices : ServiceBase, IHotelServices
    {
        private readonly IHotelRepository _repository;

        public HotelServices(IHotelRepository repository, ILogger<HotelServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<HotelModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<HotelModel>>("Consultar hoteles", null, async () =>
            {
                return await _repository.GetAllAsync();
            });
        }

        public Task<OperationResult<HotelModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<HotelModel>("Consultar hotel", null, async () =>
            {
                if (id <= 0)
                    return Failure<HotelModel>("El identificador del hotel debe ser mayor que cero.");
                return await _repository.GetByIdAsync(id);
            });
        }

        public Task<OperationResult<bool>> AddAsync(HotelAdd? hotel)
        {
            return ExecuteAsync<bool>("Registrar hotel", hotel?.CreatedUser, async () =>
            {
                if (hotel is null)
                    return Failure<bool>("El objeto HotelAdd es requerido.");

                var validation = ValidateHotel(hotel);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(hotel);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Hotel registrado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar el hotel.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(HotelUpdate? hotel)
        {
            return ExecuteAsync<bool>("Actualizar hotel", hotel?.ModifyUser, async () =>
            {
                if (hotel is null)
                    return Failure<bool>("El objeto HotelUpdate es requerido.");

                var validation = ValidateHotel(hotel);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(hotel);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Hotel actualizado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar el hotel.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(HotelRemove? hotel)
        {
            return DeleteEntityAsync(hotel, _repository.DeleteAsync, "el hotel");
        }

        // Reglas de negocio del hotel. Devuelve el mensaje de error, o null si todo está correcto.
        private static string? ValidateHotel(HotelBase hotel)
        {
            if (string.IsNullOrWhiteSpace(hotel.Nombre))
                return "El nombre del hotel es requerido.";
            if (hotel.Nombre.Trim().Length > 100)
                return "El nombre del hotel no puede superar los 100 caracteres.";
            return null;
        }
    }
}

