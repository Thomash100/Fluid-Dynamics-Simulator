using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class LocalResistanceCalculatorTests
{
    private static readonly Fluid Water = new("water", "Water", densityKilogramsPerCubicMeter: 1000);

    [Fact]
    public void CalculateZetaPressureLossPascals_UsesZetaDensityAndVelocity()
    {
        var resistance = new LocalResistance("res-1", "Resistance", zeta: 2);

        var pressureLoss = LocalResistanceCalculator.CalculateZetaPressureLossPascals(
            resistance,
            Water,
            velocityMetersPerSecond: 3);

        Assert.Equal(9000, pressureLoss);
    }

    [Fact]
    public void CalculateZetaPressureLossPascals_UsesSpeedMagnitude()
    {
        var resistance = new LocalResistance("res-1", "Resistance", zeta: 2);

        var pressureLoss = LocalResistanceCalculator.CalculateZetaPressureLossPascals(
            resistance,
            Water,
            velocityMetersPerSecond: -3);

        Assert.Equal(9000, pressureLoss);
    }

    [Fact]
    public void CalculateZetaPressureLossPascals_FromPipeUsesVolumetricFlow()
    {
        var resistance = new LocalResistance("res-1", "Resistance", zeta: 2);
        var pipe = new Pipe("pipe-1", lengthMeters: 1, innerDiameterMeters: 0.1);
        var area = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe);

        var pressureLoss = LocalResistanceCalculator.CalculateZetaPressureLossPascals(
            resistance,
            pipe,
            Water,
            volumetricFlowRateCubicMetersPerSecond: area * 3);

        Assert.Equal(9000, pressureLoss, precision: 6);
    }

    [Fact]
    public void CalculateFittingPressureLossPascals_UsesFittingResistance()
    {
        var fitting = new Fitting("fit-1", "Bend", zeta: 2, FittingKind.Bend);
        var pipe = new Pipe("pipe-1", lengthMeters: 1, innerDiameterMeters: 0.1);
        var area = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe);

        var pressureLoss = LocalResistanceCalculator.CalculateFittingPressureLossPascals(
            fitting,
            pipe,
            Water,
            volumetricFlowRateCubicMetersPerSecond: area * 3);

        Assert.Equal(9000, pressureLoss, precision: 6);
    }

    [Fact]
    public void CalculateZetaPressureLossPascals_ReturnsZeroForNoFlow()
    {
        var resistance = new LocalResistance("res-1", "Resistance", zeta: 2);

        var pressureLoss = LocalResistanceCalculator.CalculateZetaPressureLossPascals(
            resistance,
            Water,
            velocityMetersPerSecond: 0);

        Assert.Equal(0, pressureLoss);
    }

    [Fact]
    public void CalculateValvePressureLossFromKvPascals_UsesMetricKvConvention()
    {
        var coefficient = new ValveFlowCoefficient(
            kvCubicMetersPerHour: 3.6,
            kvsCubicMetersPerHour: 7.2);

        var pressureLoss = LocalResistanceCalculator.CalculateValvePressureLossFromKvPascals(
            coefficient,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0.001);

        Assert.Equal(100000, pressureLoss, precision: 6);
    }

    [Fact]
    public void CalculateValvePressureLossFromKvPascals_UsesAbsoluteFlow()
    {
        var coefficient = new ValveFlowCoefficient(
            kvCubicMetersPerHour: 3.6,
            kvsCubicMetersPerHour: 7.2);

        var pressureLoss = LocalResistanceCalculator.CalculateValvePressureLossFromKvPascals(
            coefficient,
            Water,
            volumetricFlowRateCubicMetersPerSecond: -0.001);

        Assert.Equal(100000, pressureLoss, precision: 6);
    }

    [Fact]
    public void CalculateValvePressureLossFromKvPascals_ReturnsZeroForNoFlow()
    {
        var coefficient = new ValveFlowCoefficient(
            kvCubicMetersPerHour: 3.6,
            kvsCubicMetersPerHour: 7.2);

        var pressureLoss = LocalResistanceCalculator.CalculateValvePressureLossFromKvPascals(
            coefficient,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0);

        Assert.Equal(0, pressureLoss);
    }
}
