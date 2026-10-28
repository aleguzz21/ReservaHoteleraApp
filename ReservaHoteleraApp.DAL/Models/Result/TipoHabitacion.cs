using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class TipoHabitacion : AuditEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Capacidad { get; set; }
        public decimal Precio { get; set; }

        public ICollection<Habitacion> Habitaciones { get; set; } = new List<Habitacion>();
    }
}
