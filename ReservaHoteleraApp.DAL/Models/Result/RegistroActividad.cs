using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class RegistroActividad : AuditEntity
    {
        public int IdUsuario { get; set; }
        public string Accion { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
