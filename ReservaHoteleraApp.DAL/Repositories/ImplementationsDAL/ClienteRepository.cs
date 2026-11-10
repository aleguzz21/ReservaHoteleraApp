using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ClienteRepository> logger;

        public ClienteRepository(HotelDbContext context, ILogger<ClienteRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<ClienteModel>>> GetAllAsync()
        {
            try
            {
                var clientes = await context.Clientes.AsNoTracking().ToListAsync();
                var models = clientes.Select(MapToModel).ToList();

                return new OperationResult<List<ClienteModel>>
                {
                    Data = models,
                    Message = "Clientes consultados correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar los clientes.");
                return Failure<List<ClienteModel>>("No se pudieron consultar los clientes.");
            }
        }

        public async Task<OperationResult<ClienteModel>> GetByIdAsync(int id)
        {
            try
            {
                var cliente = await context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
                if (cliente is null)
                    return Failure<ClienteModel>("El cliente no existe.");

                return new OperationResult<ClienteModel>
                {
                    Data = MapToModel(cliente),
                    Message = "Cliente consultado correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar el cliente con id {Id}.", id);
                return Failure<ClienteModel>("No se pudo consultar el cliente.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(ClienteAdd cliente)
        {
            if (cliente is null)
                return Failure<bool>("El objeto ClienteAdd es requerido.");

            var validation = ValidateCliente(cliente);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
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
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar el cliente.");
                return Failure<bool>("No se pudo registrar el cliente. Verifique datos duplicados o referencias que no existen.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar el cliente.");
                return Failure<bool>("Ocurrió un error inesperado al registrar el cliente.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(ClienteUpdate cliente)
        {
            if (cliente is null)
                return Failure<bool>("El objeto ClienteUpdate es requerido.");

            var validation = ValidateCliente(cliente);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
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
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar el cliente.");
                return Failure<bool>("No se pudo actualizar el cliente. Verifique datos duplicados o referencias que no existen.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar el cliente.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar el cliente.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(ClienteRemove cliente)
        {
            if (cliente is null)
                return Failure<bool>("El objeto ClienteRemove es requerido.");

            try
            {
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
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar el cliente.");
                return Failure<bool>("No se pudo eliminar el cliente porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar el cliente.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar el cliente.");
            }
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
