using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class ValveTests
{
    [Fact]
    public void ValveFlowCoefficient_StoresKvKvsAndOpeningRatio()
    {
        var coefficient = new ValveFlowCoefficient(
            kvCubicMetersPerHour: 2,
            kvsCubicMetersPerHour: 4);

        Assert.Equal(2, coefficient.KvCubicMetersPerHour);
        Assert.Equal(4, coefficient.KvsCubicMetersPerHour);
        Assert.Equal(0.5, coefficient.OpeningRatio);
    }

    [Fact]
    public void ValveFlowCoefficient_RejectsNonPositiveKv()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ValveFlowCoefficient(kvCubicMetersPerHour: 0, kvsCubicMetersPerHour: 4));
    }

    [Fact]
    public void ValveFlowCoefficient_RejectsKvGreaterThanKvs()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ValveFlowCoefficient(kvCubicMetersPerHour: 5, kvsCubicMetersPerHour: 4));
    }

    [Fact]
    public void Valve_StoresOptionalResistanceAndFlowCoefficient()
    {
        var resistance = new LocalResistance("valve-zeta", "Valve zeta", zeta: 4);
        var coefficient = new ValveFlowCoefficient(kvCubicMetersPerHour: 2, kvsCubicMetersPerHour: 4);

        var valve = new Valve("valve-1", "Balancing valve", resistance, coefficient);

        Assert.Equal("valve-1", valve.Id);
        Assert.Equal("Balancing valve", valve.Name);
        Assert.Same(resistance, valve.Resistance);
        Assert.Same(coefficient, valve.FlowCoefficient);
    }

    [Fact]
    public void Valve_RejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(() => new Valve("valve-1", " "));
    }
}
