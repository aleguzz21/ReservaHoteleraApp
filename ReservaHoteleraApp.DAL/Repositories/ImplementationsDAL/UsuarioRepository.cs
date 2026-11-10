using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories.ImplementationsDAL
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly HotelDbContext context;
        private readonly ILogger<UsuarioRepository> logger;

        public UsuarioRepository(HotelDbContext context, ILogger<UsuarioRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<OperationResult<List<UsuarioModel>>> GetAllAsync()
        {
            try
            {
                var usuarios = await context.Usuarios.AsNoTracking().ToListAsync();
                var models = usuarios.Select(MapToModel).ToList();

                return new OperationResult<List<UsuarioModel>>
                {
                    Data = models,
                    Message = "Usuarios consultados correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar los usuarios.");
                return Failure<List<UsuarioModel>>("No se pudieron consultar los usuarios.");
            }
        }

        public async Task<OperationResult<UsuarioModel>> GetByIdAsync(int id)
        {
            try
            {
                var usuario = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (usuario is null)
                    return Failure<UsuarioModel>("El usuario no existe.");

                return new OperationResult<UsuarioModel>
                {
                    Data = MapToModel(usuario),
                    Message = "Usuario consultado correctamente."
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar el usuario con id {Id}.", id);
                return Failure<UsuarioModel>("No se pudo consultar el usuario.");
            }
        }

        public async Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario)
        {
            if (usuario is null)
                return Failure<bool>("El objeto UsuarioAdd es requerido.");

            var validation = ValidateUsuario(usuario);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = new Usuario
                {
                    NombreUsuario = usuario.NombreUsuario!.Trim(),
                    Contrasena = usuario.Contrasena!.Trim(),
                    Rol = usuario.Rol!.Trim(),
                    Activo = usuario.Activo,
                    CreatedDate = DateTime.UtcNow,
                    CreatedUser = usuario.CreatedUser
                };

                await context.Usuarios.AddAsync(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Usuario registrado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al registrar el usuario.");
                return Failure<bool>("No se pudo registrar el usuario. Verifique datos duplicados o referencias que no existen.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al registrar el usuario.");
                return Failure<bool>("Ocurrió un error inesperado al registrar el usuario.");
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario)
        {
            if (usuario is null)
                return Failure<bool>("El objeto UsuarioUpdate es requerido.");

            var validation = ValidateUsuario(usuario);
            if (validation is not null)
                return Failure<bool>(validation);

            try
            {
                var entity = await context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);
                if (entity is null)
                    return Failure<bool>("El usuario no existe.");

                entity.NombreUsuario = usuario.NombreUsuario!.Trim();
                entity.Contrasena = usuario.Contrasena!.Trim();
                entity.Rol = usuario.Rol!.Trim();
                entity.Activo = usuario.Activo;
                entity.ModifyDate = DateTime.UtcNow;
                entity.ModifyUser = usuario.ModifyUser;

                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Usuario actualizado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al actualizar el usuario.");
                return Failure<bool>("No se pudo actualizar el usuario. Verifique datos duplicados o referencias que no existen.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al actualizar el usuario.");
                return Failure<bool>("Ocurrió un error inesperado al actualizar el usuario.");
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(UsuarioRemove usuario)
        {
            if (usuario is null)
                return Failure<bool>("El objeto UsuarioRemove es requerido.");

            try
            {
                var entity = await context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);
                if (entity is null)
                    return Failure<bool>("El usuario no existe.");

                context.Usuarios.Remove(entity);
                await context.SaveChangesAsync();

                return new OperationResult<bool>
                {
                    Data = true,
                    Message = "Usuario eliminado correctamente."
                };
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Error de base de datos al eliminar el usuario.");
                return Failure<bool>("No se pudo eliminar el usuario porque tiene registros relacionados.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al eliminar el usuario.");
                return Failure<bool>("Ocurrió un error inesperado al eliminar el usuario.");
            }
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
