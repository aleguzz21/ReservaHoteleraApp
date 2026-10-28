namespace ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion
{
    public class TipoHabitacionBase
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int Capacidad { get; set; }
        public decimal Precio { get; set; }
    }
}
