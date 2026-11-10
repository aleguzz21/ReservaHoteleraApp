using Microsoft.Extensions.Logging.Abstractions;
using ReservaHoteleraApp.BL.Services.Implementations;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Repositories.InterfacesDAL;

namespace ReservaHoteleraApp.BL.Tests;

public class UsuarioServicesTests
{
    [Fact]
    public async Task AddAsync_Null_ReturnsFailure()
    {
        var services = new UsuarioServices(new FakeUsuarioRepository(), NullLogger<UsuarioServices>.Instance);

        var result = await services.AddAsync(null);

        Assert.False(result.Success);
        Assert.Contains("requerido", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsFailure()
    {
        var services = new UsuarioServices(new FakeUsuarioRepository(), NullLogger<UsuarioServices>.Instance);

        var result = await services.GetByIdAsync(0);

        Assert.False(result.Success);
        Assert.Contains("mayor que cero", result.Message);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRepositoryFails_ReturnsFailure()
    {
        var services = new UsuarioServices(new FailingUsuarioRepository(), NullLogger<UsuarioServices>.Instance);

        var result = await services.GetByIdAsync(99);

        Assert.False(result.Success);
        Assert.Contains("no existe", result.Message);
    }

    [Fact]
    public async Task AddAsync_WhenRepositoryFails_ReturnsFailure()
    {
        var services = new UsuarioServices(new FailingUsuarioRepository(), NullLogger<UsuarioServices>.Instance);
        var usuario = new UsuarioAdd
        {
            NombreUsuario = "prueba",
            Contrasena = "123",
            Rol = "Admin"
        };

        var result = await services.AddAsync(usuario);

        Assert.False(result.Success);
        Assert.Contains("duplicados", result.Message);
    }

    private sealed class FakeUsuarioRepository : IUsuarioRepository
    {
        public Task<OperationResult<List<UsuarioModel>>> GetAllAsync()
        {
            return Task.FromResult(new OperationResult<List<UsuarioModel>> { Data = new List<UsuarioModel>() });
        }

        public Task<OperationResult<UsuarioModel>> GetByIdAsync(int id)
        {
            return Task.FromResult(OperationResult.Failure<UsuarioModel>("El usuario no existe."));
        }

        public Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario)
        {
            return Task.FromResult(OperationResult.Success(true));
        }

        public Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario)
        {
            return Task.FromResult(OperationResult.Success(true));
        }

        public Task<OperationResult<bool>> DeleteAsync(UsuarioRemove usuario)
        {
            return Task.FromResult(OperationResult.Success(true));
        }
    }

    private sealed class FailingUsuarioRepository : IUsuarioRepository
    {
        public Task<OperationResult<List<UsuarioModel>>> GetAllAsync()
        {
            return Task.FromResult(OperationResult.Failure<List<UsuarioModel>>("No se pudieron consultar los usuarios."));
        }

        public Task<OperationResult<UsuarioModel>> GetByIdAsync(int id)
        {
            return Task.FromResult(OperationResult.Failure<UsuarioModel>("El usuario no existe."));
        }

        public Task<OperationResult<bool>> AddAsync(UsuarioAdd usuario)
        {
            return Task.FromResult(OperationResult.Failure<bool>("No se pudo registrar el usuario. Verifique datos duplicados."));
        }

        public Task<OperationResult<bool>> UpdateAsync(UsuarioUpdate usuario)
        {
            return Task.FromResult(OperationResult.Failure<bool>("No se pudo actualizar el usuario."));
        }

        public Task<OperationResult<bool>> DeleteAsync(UsuarioRemove usuario)
        {
            return Task.FromResult(OperationResult.Failure<bool>("No se pudo eliminar el usuario."));
        }
    }
}
