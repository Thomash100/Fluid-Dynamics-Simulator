using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class PumpCalculatorTests
{
    private static readonly Fluid Water = new("water", "Water", densityKilogramsPerCubicMeter: 1000);

    [Fact]
    public void InterpolateHeadMeters_DelegatesToPumpCurve()
    {
        var pump = CreatePump();

        var head = PumpCalculator.InterpolateHeadMeters(pump, 0.01);

        Assert.Equal(25, head, precision: 12);
    }

    [Fact]
    public void CalculateHydraulicPowerWatts_UsesDensityGravityFlowAndHead()
    {
        var pump = CreatePump();

        var power = PumpCalculator.CalculateHydraulicPowerWatts(
            pump,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0.01);

        Assert.Equal(1000 * 9.80665 * 0.01 * 25, power, precision: 6);
    }

    [Fact]
    public void CalculateHydraulicPowerWatts_ReturnsZeroAtZeroFlow()
    {
        var pump = CreatePump();

        var power = PumpCalculator.CalculateHydraulicPowerWatts(
            pump,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0);

        Assert.Equal(0, power);
    }

    [Fact]
    public void CalculatePressureIncreasePascals_ConvertsHeadToPressure()
    {
        var pump = CreatePump();

        var pressureIncrease = PumpCalculator.CalculatePressureIncreasePascals(
            pump,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0.01);

        Assert.Equal(1000 * 9.80665 * 25, pressureIncrease, precision: 6);
    }

    [Fact]
    public void CalculateHydraulicPowerWatts_RejectsNonPositiveGravity()
    {
        var pump = CreatePump();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => PumpCalculator.CalculateHydraulicPowerWatts(
                pump,
                Water,
                volumetricFlowRateCubicMetersPerSecond: 0.01,
                gravitationalAccelerationMetersPerSecondSquared: 0));
    }

    [Fact]
    public void CalculateShaftPowerWatts_DividesHydraulicPowerByEfficiency()
    {
        var pump = CreatePump();
        var hydraulicPower = PumpCalculator.CalculateHydraulicPowerWatts(
            pump,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0.01);

        var shaftPower = PumpCalculator.CalculateShaftPowerWatts(
            pump,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0.01);

        Assert.Equal(hydraulicPower / 0.6, shaftPower, precision: 6);
    }

    [Fact]
    public void CalculateShaftPowerWatts_RequiresEfficiencyCurve()
    {
        var pump = new Pump("pump-1", "No efficiency pump", CreateCurve());

        Assert.Throws<InvalidOperationException>(
            () => PumpCalculator.CalculateShaftPowerWatts(
                pump,
                Water,
                volumetricFlowRateCubicMetersPerSecond: 0.01));
    }

    private static Pump CreatePump()
    {
        return new Pump("pump-1", "Primary pump", CreateCurve(), CreateEfficiencyCurve());
    }

    private static PumpCurve CreateCurve()
    {
        return new PumpCurve(new[]
        {
            new PumpCurvePoint(0, 30),
            new PumpCurvePoint(0.02, 20)
        });
    }

    private static PumpEfficiencyCurve CreateEfficiencyCurve()
    {
        return new PumpEfficiencyCurve(new[]
        {
            new PumpEfficiencyPoint(0, 0.4),
            new PumpEfficiencyPoint(0.02, 0.8)
        });
    }
}
