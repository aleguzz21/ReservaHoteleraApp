using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly HotelDbContext context;

        public ClienteRepository(HotelDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<Cliente>>> GetAllAsync()
        {
            var clientes = await context.Clientes.AsNoTracking().ToListAsync();

            return new OperationResult<List<Cliente>>
            {
                Data = clientes,
                Message = "Clientes consultados correctamente."
            };
        }

        public async Task<OperationResult<ClienteModel>?> GetByIdAsync(int id)
        {
            var cliente = await context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (cliente is null)
                return null;

            return new OperationResult<ClienteModel>
            {
                Data = MapToModel(cliente),
                Message = "Cliente consultado correctamente."
            };
        }

        public async Task<OperationResult<bool>> AddAsync(ClienteAdd cliente)
        {
            if (cliente is null)
                return Failure<bool>("El objeto ClienteAdd es requerido.");

            var validation = ValidateCliente(cliente);
            if (validation is not null)
                return Failure<bool>(validation);

            var entity = new Cliente
            {
                IdUsuario = cliente.IdUsuario,
                NombreCliente = cliente.NombreCliente!.Trim(),
                Documento = cliente.Documento!.Trim(),
                Correo = cliente.Correo!.Trim(),
                Telefono = cliente.Telefono?.Trim(),
                CreatedDate = DateTime.UtcNow,
                CreatedUser = cliente.CreatedUser
            };

            await context.Clientes.AddAsync(entity);
            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Cliente registrado correctamente."
            };
        }

        public async Task<OperationResult<bool>> UpdateAsync(ClienteUpdate cliente)
        {
            if (cliente is null)
                return Failure<bool>("El objeto ClienteUpdate es requerido.");

            var validation = ValidateCliente(cliente);
            if (validation is not null)
                return Failure<bool>(validation);

            var entity = await context.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id);
            if (entity is null)
                return Failure<bool>("El cliente no existe.");

            entity.IdUsuario = cliente.IdUsuario;
            entity.NombreCliente = cliente.NombreCliente!.Trim();
            entity.Documento = cliente.Documento!.Trim();
            entity.Correo = cliente.Correo!.Trim();
            entity.Telefono = cliente.Telefono?.Trim();
            entity.ModifyDate = DateTime.UtcNow;
            entity.ModifyUser = cliente.ModifyUser;

            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Cliente actualizado correctamente."
            };
        }

        public async Task<OperationResult<bool>> DeleteAsync(ClienteRemove cliente)
        {
            if (cliente is null)
                return Failure<bool>("El objeto ClienteRemove es requerido.");

            var entity = await context.Clientes.FirstOrDefaultAsync(c => c.Id == cliente.Id);
            if (entity is null)
                return Failure<bool>("El cliente no existe.");

            context.Clientes.Remove(entity);
            await context.SaveChangesAsync();

            return new OperationResult<bool>
            {
                Data = true,
                Message = "Cliente eliminado correctamente."
            };
        }

        private static string? ValidateCliente(ClienteBase cliente)
        {
            if (cliente.IdUsuario <= 0)
                return "El identificador del usuario debe ser mayor que cero.";
            if (string.IsNullOrWhiteSpace(cliente.NombreCliente))
                return "El nombre del cliente es requerido.";
            if (cliente.NombreCliente.Trim().Length > 150)
                return "El nombre del cliente no puede superar los 150 caracteres.";
            if (string.IsNullOrWhiteSpace(cliente.Documento))
                return "El documento es requerido.";
            if (cliente.Documento.Trim().Length > 30)
                return "El documento no puede superar los 30 caracteres.";
            if (string.IsNullOrWhiteSpace(cliente.Correo))
                return "El correo es requerido.";
            if (cliente.Correo.Trim().Length > 150)
                return "El correo no puede superar los 150 caracteres.";
            if (!cliente.Correo.Contains('@'))
                return "El correo no tiene un formato válido.";
            if (!string.IsNullOrWhiteSpace(cliente.Telefono) && cliente.Telefono.Trim().Length > 25)
                return "El teléfono no puede superar los 25 caracteres.";

            return null;
        }

        private static ClienteModel MapToModel(Cliente cliente)
        {
            return new ClienteModel
            {
                Id = cliente.Id,
                IdUsuario = cliente.IdUsuario,
                NombreCliente = cliente.NombreCliente,
                Documento = cliente.Documento,
                Correo = cliente.Correo,
                Telefono = cliente.Telefono
            };
        }
    }
}
