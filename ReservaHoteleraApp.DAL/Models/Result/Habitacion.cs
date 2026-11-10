using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class Habitacion : AuditEntity
    {
        public string Numero { get; set; } = string.Empty;
        public int IdPiso { get; set; }
        public int IdTipoHabitacion { get; set; }
        public decimal PrecioNoche { get; set; }
        public string Estado { get; set; } = "Disponible";

        public Piso? Piso { get; set; }
        public TipoHabitacion? TipoHabitacion { get; set; }
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
