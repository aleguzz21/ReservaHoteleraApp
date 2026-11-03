using ReservaHoteleraApp.DAL.Models.Base;
namespace ReservaHoteleraApp.DAL.Models.Objects.Estadia
{
    public class EstadiaRemove : IRemoveRequest
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
