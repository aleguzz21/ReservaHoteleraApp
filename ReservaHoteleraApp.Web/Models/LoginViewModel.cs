using System.ComponentModel.DataAnnotations;



namespace ReservaHoteleraApp.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario requerido")]
        public string NombreUsuario { get; set; } = "";
        [Required(ErrorMessage ="La contrasena es requerida")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = "";
        public bool Recordarme { get; set; }
    }
}
