namespace ReservaHoteleraApp.DAL.Models.Objects.Usuario
{
    public class UsuarioBase
    {
        public string? NombreUsuario { get; set; }
        public string? Contrasena { get; set; }
        public string? Rol { get; set; }
        public bool Activo { get; set; } = true;
    }
}
