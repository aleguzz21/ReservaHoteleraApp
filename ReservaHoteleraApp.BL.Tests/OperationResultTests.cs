using ReservaHoteleraApp.DAL.Models;

namespace ReservaHoteleraApp.BL.Tests;

public class OperationResultTests
{
    [Fact]
    public void Success_SetsDataAndSuccess()
    {
        var result = OperationResult.Success("ok", "Operación correcta.");

        Assert.True(result.Success);
        Assert.Equal("ok", result.Data);
        Assert.Equal("Operación correcta.", result.Message);
    }

    [Fact]
    public void Failure_SetsMessageAndSuccessFalse()
    {
        var result = OperationResult.Failure<bool>("No fue posible completar la operación.");

        Assert.False(result.Success);
        Assert.False(result.Data);
        Assert.Equal("No fue posible completar la operación.", result.Message);
    }
}
