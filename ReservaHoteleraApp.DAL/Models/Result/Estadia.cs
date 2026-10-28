using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Result
{
    public sealed class Estadia : EstanciaEntity
    {
        public int IdReserva { get; set; }
        public int IdUsuario { get; set; }

        public Reserva? Reserva { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
