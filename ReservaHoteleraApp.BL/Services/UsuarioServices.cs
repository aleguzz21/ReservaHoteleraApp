using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories;
using static ReservaHoteleraApp.DAL.Models.OperationResult;


namespace ReservaHoteleraApp.BL.Services
{
    public class UsuarioServices : ServiceBase , IUsuarioServices
    {
        private readonly IUsuarioRepository _repository;
        
        public UsuarioServices(IUsuarioRepository repository, ILogger<UsuarioServices> logger) : base(logger)
        {
            _repository = repository;
           
        }

        public Task<OperationResult<List<UsuarioModel>>> GetAllAsync()
        {
            return ExecuteAsync<List<UsuarioModel>>("Consultar usuarios", null, async () =>
            {
                var result = await _repository.GetAllAsync();
                if (!result.Success || result.Data is null)
                    return Failure<List<UsuarioModel>>(result.Message ?? "No fue posible consultar los usuarios.");

                var models = result.Data.Select(MapToModel).ToList();
                return new OperationResult<List<UsuarioModel>>
                {
                    Data = models,
                    Message = "Usuarios consultados correctamente."
                };
            });
        }

        public Task<OperationResult<UsuarioModel>> GetByIdAsync(int id)
        {
            return ExecuteAsync<UsuarioModel>("Consultar usuario", null, async () =>
            {
                if (id <= 0)
                    return Failure<UsuarioModel>("El identificador del usuario debe ser mayor que cero.");

                var result = await _repository.GetByIdAsync(id);
                if (result is null || !result.Success || result.Data is null)
                    return Failure<UsuarioModel>("El usuario no existe.");

                return result;
            });
        }

        public Task<OperationResult<bool>> AddAsync(UsuarioAdd? usuario)
        {
            return ExecuteAsync<bool>("Registrar usuario", usuario?.CreatedUser, async () =>
            {
                if (usuario is null)
                    return Failure<bool>("El objeto UsuarioAdd es requerido.");

                var validation = ValidateUsuario(usuario);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.AddAsync(usuario);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Usuario registrado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible registrar el usuario.");
            });
        }

        public Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate? usuario)
        {
            return ExecuteAsync<bool>("Actualizar usuario", usuario?.ModifyUser, async () =>
            {
                if (usuario is null)
                    return Failure<bool>("El objeto UsuarioUpdate es requerido.");

                var validation = ValidateUsuario(usuario);
                if (validation is not null)
                    return Failure<bool>(validation);

                var result = await _repository.UpdateAsync(usuario);
                return result.Success && result.Data
                    ? new OperationResult<bool> { Data = true, Message = "Usuario actualizado correctamente." }
                    : Failure<bool>(result.Message ?? "No fue posible actualizar el usuario.");
            });
        }

        public Task<OperationResult<bool>> DeleteAsync(UsuarioRemove? usuario) {
            return DeleteEntityAsync(usuario, _repository.DeleteAsync, "el usuario");
        }

           private static string? ValidateUsuario(UsuarioBase usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
                return "El nombre de usuario es requerido.";
            if (usuario.NombreUsuario.Trim().Length > 50)
                return "El nombre de usuario no puede superar los 50 caracteres.";
            if (string.IsNullOrWhiteSpace(usuario.Contrasena))
                return "La contraseña es requerida.";
            if (usuario.Contrasena.Trim().Length > 256)
                return "La contraseña no puede superar los 256 caracteres.";
            if (string.IsNullOrWhiteSpace(usuario.Rol))
                return "El rol es requerido.";
            if (usuario.Rol.Trim().Length > 30)
                return "El rol no puede superar los 30 caracteres.";

            return null;
        }



        private static UsuarioModel MapToModel(Usuario usuario)
        {
            return new UsuarioModel
            {
                Id = usuario.Id,
                NombreUsuario = usuario.NombreUsuario,
                Contrasena = usuario.Contrasena,
                Rol = usuario.Rol,
                Activo = usuario.Activo
            };
        }
    }
}
