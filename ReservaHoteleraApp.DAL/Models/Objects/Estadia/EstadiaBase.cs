namespace ReservaHoteleraApp.DAL.Models.Objects.Estadia
{
    public class EstadiaBase
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaEntrada { get; set; }
        public DateTime? FechaSalida { get; set; }
    }
}
