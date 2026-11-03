using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.BL.Services
{
    public class ClienteServices : ServiceBase, IClienteServices
    {
        private readonly IClienteRepository _repository;

        public ClienteServices(IClienteRepository repository, ILogger<ClienteServices> logger)
            : base(logger)
        {
            _repository = repository;
        }

        public Task<OperationResult<List<ClienteModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<ClienteModel>>("Consultar clientes", null, async () =>
            {
                var result = await _repository.GetAllAsync();
                if (!result.Success || result.Data is null)
                    return Failure<List<ClienteModel>>(result.Message ?? "No fue posible consultar los clientes.");

                var models = result.Data.Select(MapToModel).ToList();
                return new OperationResult<List<ClienteModel>>
                {
                    Data = models,
                    Message = "Clientes consultados correctamente."
                };
            });
        }

        public Task<OperationResult<ClienteModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<ClienteModel>("Consultar cliente", null, async () =>
            {
                if (id <= 0)
                    return Failure<ClienteModel>("El identificador del cliente debe ser mayor que cero.");

                var result = await _repository.GetByIdAsync(id);
                if (result is null || !result.Success || result.Data is null)
                    return Failure<ClienteModel>("El cliente no existe.");

                return result;
            });
        }

        public Task<OperationResult<bool>> AddAsync(ClienteAdd? cliente)
        {
            return ExecuteAsync<bool>("Registrar cliente", cliente?.CreatedUser, async () =>
            {
                if (cliente is null)
                    return Failure<bool>("El objeto ClienteAdd es requerido.");

                var validation = ValidateCliente(cliente);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(cliente);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Cliente registrado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar el cliente.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(ClienteUpdate? cliente)
        {
            return ExecuteAsync<bool>("Actualizar cliente", cliente?.ModifyUser, async () =>
            {
                if (cliente is null)
                    return Failure<bool>("El objeto ClienteUpdate es requerido.");

                var validation = ValidateCliente(cliente);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(cliente);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Cliente actualizado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar el cliente.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(ClienteRemove? cliente)
        {
            return DeleteEntityAsync(cliente, _repository.DeleteAsync, "el cliente");
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
