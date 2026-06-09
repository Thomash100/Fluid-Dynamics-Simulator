using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class PipeFlowCalculatorTests
{
    private static readonly Fluid Water = new("water", "Water", densityKilogramsPerCubicMeter: 1000);

    [Fact]
    public void CalculateCrossSectionalAreaSquareMeters_UsesInnerDiameter()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1);

        var area = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe);

        Assert.Equal(Math.PI * 0.05 * 0.05, area, precision: 12);
    }

    [Fact]
    public void CalculateVelocityMetersPerSecond_ConvertsVolumetricFlowToVelocity()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1);
        var area = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe);

        var velocity = PipeFlowCalculator.CalculateVelocityMetersPerSecond(
            pipe,
            volumetricFlowRateCubicMetersPerSecond: area * 2);

        Assert.Equal(2, velocity, precision: 12);
    }

    [Fact]
    public void CalculateVelocityMetersPerSecond_PreservesFlowDirectionSign()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1);
        var area = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe);

        var velocity = PipeFlowCalculator.CalculateVelocityMetersPerSecond(
            pipe,
            volumetricFlowRateCubicMetersPerSecond: -area * 2);

        Assert.Equal(-2, velocity, precision: 12);
    }

    [Fact]
    public void CalculateReynoldsNumber_UsesDensityVelocityDiameterAndDynamicViscosity()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.05);
        var area = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe);

        var reynoldsNumber = PipeFlowCalculator.CalculateReynoldsNumber(
            pipe,
            Water,
            volumetricFlowRateCubicMetersPerSecond: area * 2,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(100000, reynoldsNumber, precision: 6);
    }

    [Fact]
    public void CalculateReynoldsNumber_UsesSpeedMagnitude()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.05);
        var area = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe);

        var reynoldsNumber = PipeFlowCalculator.CalculateReynoldsNumber(
            pipe,
            Water,
            volumetricFlowRateCubicMetersPerSecond: -area * 2,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(100000, reynoldsNumber, precision: 6);
    }

    [Fact]
    public void CalculateReynoldsNumber_RejectsNonPositiveDynamicViscosity()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.05);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => PipeFlowCalculator.CalculateReynoldsNumber(pipe, Water, 0.01, dynamicViscosityPascalSeconds: 0));
    }

    [Fact]
    public void EstimateDarcyFrictionFactor_UsesLaminarFormula()
    {
        var frictionFactor = PipeFlowCalculator.EstimateDarcyFrictionFactor(1000);

        Assert.Equal(0.064, frictionFactor, precision: 12);
    }

    [Fact]
    public void EstimateDarcyFrictionFactor_UsesSimpleBlasiusApproximationOutsideLaminarRange()
    {
        var frictionFactor = PipeFlowCalculator.EstimateDarcyFrictionFactor(100000);

        Assert.Equal(0.3164 / Math.Pow(100000, 0.25), frictionFactor, precision: 12);
    }

    [Fact]
    public void EstimateDarcyFrictionFactor_RejectsZeroReynoldsNumber()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => PipeFlowCalculator.EstimateDarcyFrictionFactor(0));
    }

    [Fact]
    public void CalculateDarcyWeisbachPressureLossPascals_ReturnsPositiveSinglePipeLoss()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.05);
        var area = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe);
        var volumetricFlowRate = area * 2;
        var reynoldsNumber = PipeFlowCalculator.CalculateReynoldsNumber(pipe, Water, volumetricFlowRate, 0.001);
        var frictionFactor = PipeFlowCalculator.EstimateDarcyFrictionFactor(reynoldsNumber);
        var expectedPressureLoss = frictionFactor * (10 / 0.05) * (1000 * 2 * 2 / 2);

        var pressureLoss = PipeFlowCalculator.CalculateDarcyWeisbachPressureLossPascals(
            pipe,
            Water,
            volumetricFlowRate,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(expectedPressureLoss, pressureLoss, precision: 6);
    }

    [Fact]
    public void CalculateDarcyWeisbachPressureLossPascals_ReturnsZeroForNoFlow()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.05);

        var pressureLoss = PipeFlowCalculator.CalculateDarcyWeisbachPressureLossPascals(
            pipe,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(0, pressureLoss);
    }
}
