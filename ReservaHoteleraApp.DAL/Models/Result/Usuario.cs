using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class Usuario : AuditEntity
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;

        public Cliente? Cliente { get; set; }
        public ICollection<Estadia> Estadias { get; set; } = new List<Estadia>();
        public ICollection<RegistroActividad> RegistrosActividad { get; set; } = new List<RegistroActividad>();
    }
}
