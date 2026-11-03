using ReservaHoteleraApp.DAL.Models.Base;
namespace ReservaHoteleraApp.DAL.Models.Objects.Pago
{
    public class PagoRemove : IRemoveRequest
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
