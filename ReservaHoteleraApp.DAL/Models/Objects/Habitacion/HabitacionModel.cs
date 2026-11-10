namespace ReservaHoteleraApp.DAL.Models.Objects.Habitacion
{
    public class HabitacionModel : HabitacionBase
    {
        public int Id { get; set; }
        public int? PisoNumero { get; set; }
        public string? TipoHabitacionNombre { get; set; }
    }
}
