using ReservaHoteleraApp.DAL.Models.Base;
namespace ReservaHoteleraApp.DAL.Models.Objects.Habitacion
{
    public class HabitacionRemove : IRemoveRequest
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
