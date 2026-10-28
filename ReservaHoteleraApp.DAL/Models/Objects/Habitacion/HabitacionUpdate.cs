namespace ReservaHoteleraApp.DAL.Models.Objects.Habitacion
{
    public class HabitacionUpdate : HabitacionBase
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
