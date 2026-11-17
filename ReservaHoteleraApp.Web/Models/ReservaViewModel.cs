using System.ComponentModel.DataAnnotations;

namespace ReservaHoteleraApp.Web.Models
{
    public class ReservaViewModel
    {
        public int IdHabitacion { get; set; }
        public string? NumeroHabitacion { get; set; }
        public string? HotelNombre { get; set; }
        public int? PisoNumero { get; set; }
        public string? TipoHabitacionNombre { get; set; }
        public decimal PrecioNoche { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaEntrada { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime FechaSalida { get; set; } = DateTime.Today.AddDays(1);

        [Range(1, 20, ErrorMessage = "Debe indicar entre 1 y 20 personas.")]
        public int CantidadPersonas { get; set; } = 1;
    }
}
