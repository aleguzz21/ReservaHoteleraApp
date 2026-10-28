namespace ReservaHoteleraApp.DAL.Models.Objects.Habitacion
{
    public class HabitacionBase
    {
        public string? Numero { get; set; }
        public int IdTipoHabitacion { get; set; }
        public decimal PrecioNoche { get; set; }
        public string? Estado { get; set; } = "Disponible";
    }
}
