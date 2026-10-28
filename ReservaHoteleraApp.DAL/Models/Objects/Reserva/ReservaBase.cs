namespace ReservaHoteleraApp.DAL.Models.Objects.Reserva
{
    public class ReservaBase
    {
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public int CantidadPersonas { get; set; }
        public string? Estado { get; set; } = "Pendiente";
        public decimal TotalReserva { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
    }
}
