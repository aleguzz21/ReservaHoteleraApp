using ReservaHoteleraApp.DAL.Models.Base;
namespace ReservaHoteleraApp.DAL.Models.Objects.Usuario

{
    public class UsuarioRemove: IRemoveRequest
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
