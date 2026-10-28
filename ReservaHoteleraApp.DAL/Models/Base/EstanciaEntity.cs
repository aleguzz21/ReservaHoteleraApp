namespace ReservaHoteleraApp.DAL.Models.Base
{
    public abstract class EstanciaEntity : AuditEntity
    {
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
    }
}
