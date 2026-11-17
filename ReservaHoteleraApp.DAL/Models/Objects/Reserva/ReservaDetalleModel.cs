namespace ReservaHoteleraApp.DAL.Models.Objects.Reserva
{
    // Reserva con los datos del alojamiento ya "aplanados" para mostrarlos en pantalla.
    public class ReservaDetalleModel : ReservaBase
    {
        public int Id { get; set; }
        public string? ClienteNombre { get; set; }
        public string? HotelNombre { get; set; }
        public string? Ciudad { get; set; }
        public string? BloqueNombre { get; set; }
        public int? PisoNumero { get; set; }
        public string? NumeroHabitacion { get; set; }
        public string? TipoHabitacionNombre { get; set; }
        public decimal PrecioNoche { get; set; }

        public int Noches
        {
            get
            {
                if (FechaSalida is null)
                    return 0;

                return (FechaSalida.Value - FechaEntrada).Days;
            }
        }
    }
}
