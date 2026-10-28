namespace ReservaHoteleraApp.DAL.Models.Objects.Pago
{
    public class PagoBase
    {
        public int IdReserva { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string? MetodoPago { get; set; }
    }
}
