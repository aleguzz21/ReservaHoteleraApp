using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class Reserva : EstanciaEntity
    {
        public int IdCliente { get; set; }
        public int IdHabitacion { get; set; }
        public int CantidadPersonas { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public decimal TotalReserva { get; set; }

        public Cliente? Cliente { get; set; }
        public Habitacion? Habitacion { get; set; }
        public Estadia? Estadia { get; set; }
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
