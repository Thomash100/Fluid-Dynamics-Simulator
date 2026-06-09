using FDS.Core.Models;

namespace FDS.Core.Tests;

public sealed class NodeTests
{
    [Fact]
    public void Constructor_StoresIdPressureAndTemperatureUnits()
    {
        var temperature = Temperature.FromCelsius(21.5);

        var node = new Node("node-1", "Supply", pressurePascals: 101325, temperature);

        Assert.Equal("node-1", node.Id);
        Assert.Equal("Supply", node.Name);
        Assert.Equal(101325, node.PressurePascals);
        Assert.NotNull(node.Temperature);
        Assert.Equal(21.5, node.Temperature.Value.Celsius, precision: 6);
        Assert.Equal(294.65, node.Temperature.Value.Kelvin, precision: 6);
    }

    [Fact]
    public void Constructor_RejectsEmptyId()
    {
        Assert.Throws<ArgumentException>(() => new Node(" "));
    }

    [Fact]
    public void Constructor_RejectsNonFinitePressure()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Node("node-1", pressurePascals: double.NaN));
    }
}
