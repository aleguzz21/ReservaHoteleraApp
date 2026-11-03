using ReservaHoteleraApp.DAL.Models.Base;
namespace ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion
{
    public class TipoHabitacionRemove : IRemoveRequest
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
