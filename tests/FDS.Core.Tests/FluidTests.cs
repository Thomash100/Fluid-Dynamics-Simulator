using FDS.Core.Models;

namespace FDS.Core.Tests;

public sealed class FluidTests
{
    [Fact]
    public void Constructor_StoresDensityAndReferenceTemperatureUnits()
    {
        var fluid = new Fluid(
            "water",
            "Water",
            densityKilogramsPerCubicMeter: 998.2,
            referenceTemperature: Temperature.FromCelsius(20));

        Assert.Equal("water", fluid.Id);
        Assert.Equal("Water", fluid.Name);
        Assert.Equal(998.2, fluid.DensityKilogramsPerCubicMeter);
        Assert.NotNull(fluid.ReferenceTemperature);
        Assert.Equal(20, fluid.ReferenceTemperature.Value.Celsius, precision: 6);
        Assert.Equal(293.15, fluid.ReferenceTemperature.Value.Kelvin, precision: 6);
    }

    [Fact]
    public void Constructor_AllowsZeroButRejectsNegativeDensity()
    {
        var fluid = new Fluid("test", "Test Fluid", densityKilogramsPerCubicMeter: 0);

        Assert.Equal(0, fluid.DensityKilogramsPerCubicMeter);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Fluid("invalid", "Invalid", densityKilogramsPerCubicMeter: -1));
    }

    [Fact]
    public void Constructor_RejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(
            () => new Fluid("fluid-1", " ", densityKilogramsPerCubicMeter: 1));
    }
}
