using Microsoft.EntityFrameworkCore;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Models.Result;
using static ReservaHoteleraApp.DAL.Models.OperationResult;

namespace ReservaHoteleraApp.DAL.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly HotelDbContext context;

        public UsuarioRepository(HotelDbContext context)
        {
            this.context = context;
        }

        public async Task<OperationResult<List<Usuario>>> GetAllAsync()
        {
            var usuarios = await context.Usuarios.AsNoTracking().ToListAsync();

            return new OperationResult<List<Usuario>>
            {
                Data = usuarios,
                Message = "Usuarios consultados correctamente."
            };
        }

        public async Task<OperationResult<UsuarioModel>?> GetByIdAsync(int id)
        {
            var usuario = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (usuario is null)
                return null;

            return new OperationResult<UsuarioModel>
            {
                Data = MapToModel(usuario),
                Message = "Usuario consultado correctamente."
            };
        }

        public async Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario)
        {
            if (usuario is null)
                return Failure<bool>("El objeto UsuarioAdd es requerido.");

            var validation = ValidateUsuario(usuario);
            if (validation is not null)
                return Failure<bool>(validation);

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

        public async Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario)
        {
            if (usuario is null)
                return Failure<bool>("El objeto UsuarioUpdate es requerido.");

            var validation = ValidateUsuario(usuario);
            if (validation is not null)
                return Failure<bool>(validation);

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

        public async Task<OperationResult<bool>> DeleteAsync(UsuarioRemove usuario)
        {
            if (usuario is null)
                return Failure<bool>("El objeto UsuarioRemove es requerido.");

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
