using System.ComponentModel.DataAnnotations;

namespace ReservaHoteleraApp.Web.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        public string NombreUsuario { get; set; } = "";

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = "";

        [DataType(DataType.Password)]
        [Compare(nameof(Contrasena), ErrorMessage ="Las contrasena no coinciden.")]
        public string ConfirmarContrasena { get; set; } = "";

        [Required, StringLength(150)]
        public string NombreCliente { get; set; } = "";

        [Required, StringLength(30)]
        public string Documento { get; set; } = "";

        [Required, EmailAddress, StringLength(150)]
        public string Correo { get; set; } = "";

        [StringLength(25)]
        public string? Telefono { get; set; } = "";

    }
}
