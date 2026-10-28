namespace ReservaHoteleraApp.DAL.Models.Objects.Cliente
{
    public class ClienteBase
    {
        public int IdUsuario { get; set; }
        public string? NombreCliente { get; set; }
        public string? Documento { get; set; }
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
    }
}
