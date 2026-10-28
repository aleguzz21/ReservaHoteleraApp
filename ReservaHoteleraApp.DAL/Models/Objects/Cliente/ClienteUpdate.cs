namespace ReservaHoteleraApp.DAL.Models.Objects.Cliente
{
    public class ClienteUpdate : ClienteBase
    {
        public int Id { get; set; }
        public int? ModifyUser { get; set; }
    }
}
