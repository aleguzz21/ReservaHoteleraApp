using Microsoft.Extensions.Logging.Abstractions;
using ReservaHoteleraApp.BL.Services.Implementations;
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Models.Result;
using ReservaHoteleraApp.DAL.Repositories;

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

    private sealed class FakeUsuarioRepository : IUsuarioRepository
    {
        public Task<OperationResult<List<Usuario>>> GetAllAsync()
        {
            return Task.FromResult(new OperationResult<List<Usuario>> { Data = new List<Usuario>() });
        }

        public Task<OperationResult<UsuarioModel>?> GetByIdAsync(int id)
        {
            return Task.FromResult<OperationResult<UsuarioModel>?>(null);
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
}
