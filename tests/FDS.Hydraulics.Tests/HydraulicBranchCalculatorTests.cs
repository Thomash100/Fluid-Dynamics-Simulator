using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class HydraulicBranchCalculatorTests
{
    private static readonly Fluid Water = new("water", "Water", densityKilogramsPerCubicMeter: 1000);

    [Fact]
    public void Calculate_SumsPipeLossesLocalLossesPumpIncreaseAndNetBalance()
    {
        var referencePipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1);
        var secondPipe = new Pipe("pipe-2", lengthMeters: 5, innerDiameterMeters: 0.1);
        var volumetricFlowRate = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(referencePipe) * 2;
        var pump = new Pump(
            "pump-1",
            "Pump",
            new PumpCurve(new[]
            {
                new PumpCurvePoint(0, 5),
                new PumpCurvePoint(0.02, 5)
            }));
        var branch = new HydraulicBranch(
            "branch-1",
            "Primary branch",
            new[] { referencePipe, secondPipe },
            new[] { new LocalResistance("res-1", "Resistance", zeta: 1) },
            new[] { new Fitting("fit-1", "Bend", zeta: 2, FittingKind.Bend) },
            new[] { new Valve("valve-1", "Valve", new LocalResistance("valve-zeta", "Valve zeta", zeta: 3)) },
            pump);

        var result = HydraulicBranchCalculator.Calculate(
            branch,
            Water,
            volumetricFlowRate,
            dynamicViscosityPascalSeconds: 0.001);

        var expectedPipeLoss =
            PipeFlowCalculator.CalculateDarcyWeisbachPressureLossPascals(referencePipe, Water, volumetricFlowRate, 0.001)
            + PipeFlowCalculator.CalculateDarcyWeisbachPressureLossPascals(secondPipe, Water, volumetricFlowRate, 0.001);
        var expectedLocalLoss = 12000d;
        var expectedPumpIncrease = 1000 * 9.80665 * 5;

        Assert.Equal(volumetricFlowRate, result.VolumetricFlowRateCubicMetersPerSecond, precision: 12);
        Assert.Equal(expectedPipeLoss, result.PipePressureLossPascals, precision: 6);
        Assert.Equal(expectedLocalLoss, result.LocalPressureLossPascals, precision: 6);
        Assert.Equal(expectedPumpIncrease, result.PumpPressureIncreasePascals, precision: 6);
        Assert.Equal(
            expectedPumpIncrease - expectedPipeLoss - expectedLocalLoss,
            result.NetPressureBalancePascals,
            precision: 6);
    }

    [Fact]
    public void Calculate_UsesValveFlowCoefficientWhenAvailable()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 0, innerDiameterMeters: 0.1);
        var valve = new Valve(
            "valve-1",
            "Balancing valve",
            new LocalResistance("valve-zeta", "Valve zeta", zeta: 999),
            new ValveFlowCoefficient(kvCubicMetersPerHour: 3.6, kvsCubicMetersPerHour: 7.2));
        var branch = new HydraulicBranch("branch-1", "Branch", new[] { pipe }, valves: new[] { valve });

        var result = HydraulicBranchCalculator.Calculate(
            branch,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0.001,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(100000, result.LocalPressureLossPascals, precision: 6);
    }

    [Fact]
    public void Calculate_ReturnsNegativeNetBalanceWithoutPump()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 0, innerDiameterMeters: 0.1);
        var branch = new HydraulicBranch(
            "branch-1",
            "Branch",
            new[] { pipe },
            localResistances: new[] { new LocalResistance("res-1", "Resistance", zeta: 1) });
        var volumetricFlowRate = PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe) * 2;

        var result = HydraulicBranchCalculator.Calculate(
            branch,
            Water,
            volumetricFlowRate,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(0, result.PumpPressureIncreasePascals);
        Assert.Equal(-2000, result.NetPressureBalancePascals, precision: 6);
    }

    [Fact]
    public void Calculate_ReturnsZeroLossesForNoFlowWithoutPump()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1);
        var branch = new HydraulicBranch(
            "branch-1",
            "Branch",
            new[] { pipe },
            localResistances: new[] { new LocalResistance("res-1", "Resistance", zeta: 1) });

        var result = HydraulicBranchCalculator.Calculate(
            branch,
            Water,
            volumetricFlowRateCubicMetersPerSecond: 0,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(0, result.PipePressureLossPascals);
        Assert.Equal(0, result.LocalPressureLossPascals);
        Assert.Equal(0, result.NetPressureBalancePascals);
    }

    [Fact]
    public void Calculate_RejectsNegativeFlow()
    {
        var branch = new HydraulicBranch(
            "branch-1",
            "Branch",
            new[] { new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1) });

        Assert.Throws<ArgumentOutOfRangeException>(
            () => HydraulicBranchCalculator.Calculate(
                branch,
                Water,
                volumetricFlowRateCubicMetersPerSecond: -0.001,
                dynamicViscosityPascalSeconds: 0.001));
    }

    [Fact]
    public void Calculate_RejectsNonPositiveDynamicViscosity()
    {
        var branch = new HydraulicBranch(
            "branch-1",
            "Branch",
            new[] { new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1) });

        Assert.Throws<ArgumentOutOfRangeException>(
            () => HydraulicBranchCalculator.Calculate(
                branch,
                Water,
                volumetricFlowRateCubicMetersPerSecond: 0.001,
                dynamicViscosityPascalSeconds: 0));
    }
}
