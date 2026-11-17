namespace ReservaHoteleraApp.DAL.Models.Objects.Habitacion
{
    public class HabitacionModel : HabitacionBase
    {
        public int Id { get; set; }
        public int? IdHotel { get; set; }
        public int? PisoNumero { get; set; }
        public string? TipoHabitacionNombre { get; set; }
        public string? BloqueNombre { get; set; }
        public string? HotelNombre { get; set; }
        public string? Ciudad { get; set; }
    }
}
