using ReservaHoteleraApp.DAL.Models.Base;

namespace ReservaHoteleraApp.DAL.Models.Objects.Bloque
{
    public class BloqueRemove : IRemoveRequest
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
