using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ReservaHoteleraApp.DAL.Models.Objects.Bloque;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;
using ReservaHoteleraApp.DAL.Models.Objects.Piso;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;

namespace ReservaHoteleraApp.Web.Models
{
    // Modelo de la página única "Configurar hotel".
    // Reúne el hotel y todo lo que depende de él (bloques, pisos y habitaciones)
    // más el catálogo global de tipos de habitación.
    public class HotelConfiguracionViewModel
    {
        public HotelModel Hotel { get; set; } = new HotelModel();

        public List<BloqueModel> Bloques { get; set; } = new List<BloqueModel>();
        public List<PisoModel> Pisos { get; set; } = new List<PisoModel>();
        public List<TipoHabitacionModel> Tipos { get; set; } = new List<TipoHabitacionModel>();
        public List<HabitacionModel> Habitaciones { get; set; } = new List<HabitacionModel>();

        // Formularios de cada sección. Si Id > 0 el formulario está en modo edición.
        public BloqueConfigForm BloqueForm { get; set; } = new BloqueConfigForm();
        public PisoConfigForm PisoForm { get; set; } = new PisoConfigForm();
        public TipoHabitacionConfigForm TipoForm { get; set; } = new TipoHabitacionConfigForm();
        public HabitacionConfigForm HabitacionForm { get; set; } = new HabitacionConfigForm();

        // Indica qué sección se está editando ("bloque", "piso", "tipo" o "habitacion").
        public string? SeccionEdicion { get; set; }
    }

    public class BloqueConfigForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del bloque es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string? Nombre { get; set; }

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }
    }

    public class PisoConfigForm
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El número del piso debe ser mayor que cero.")]
        public int Numero { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un bloque.")]
        public int IdBloque { get; set; }

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }
    }

    public class TipoHabitacionConfigForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del tipo es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string? Nombre { get; set; }

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor que cero.")]
        public int Capacidad { get; set; }

        [Range(typeof(decimal), "0", "999999999999", ErrorMessage = "El precio no puede ser negativo.")]
        public decimal Precio { get; set; }
    }

    public class HabitacionConfigForm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de habitación es requerido.")]
        [StringLength(10, ErrorMessage = "El número no puede superar los 10 caracteres.")]
        public string? Numero { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un piso.")]
        public int IdPiso { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de habitación.")]
        public int IdTipoHabitacion { get; set; }

        [Range(typeof(decimal), "0", "999999999999", ErrorMessage = "El precio por noche no puede ser negativo.")]
        public decimal PrecioNoche { get; set; }

        [StringLength(20, ErrorMessage = "El estado no puede superar los 20 caracteres.")]
        public string? Estado { get; set; } = "Disponible";
    }
}
