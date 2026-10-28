namespace ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad
{
    public class RegistroActividadBase
    {
        public int IdUsuario { get; set; }
        public string? Accion { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
