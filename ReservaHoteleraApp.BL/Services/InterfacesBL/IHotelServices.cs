using System;
using System.Collections.Generic;
using System.Text;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;

namespace ReservaHoteleraApp.BL.Services.Interfaces
{
    public interface IHotelServices
    {
        Task<OperationResult<List<HotelModel>>> GetAllAsync();
        Task<OperationResult<HotelModel>> GetByIdAsync(int id);
        Task<OperationResult<bool>> AddAsync(HotelAdd? hotel);
        Task<OperationResult<bool>> UpdateAsync(HotelUpdate? hotel);
        Task<OperationResult<bool>> DeleteAsync(HotelRemove? hotel);
    }
}
