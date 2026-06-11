using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class HydraulicNetworkCalculatorTests
{
    private static readonly Fluid Water = new("water", "Water", densityKilogramsPerCubicMeter: 1000);

    [Fact]
    public void Calculate_EvaluatesSingleBranch()
    {
        var branch = CreateBranch("branch-1", zeta: 1);
        var flow = CreateFlowForVelocity(branch, velocityMetersPerSecond: 2);
        var network = new HydraulicNetwork(
            "network-1",
            "Network",
            new[] { new HydraulicBranchFlow(branch, flow) });

        var result = HydraulicNetworkCalculator.Calculate(
            network,
            Water,
            dynamicViscosityPascalSeconds: 0.001);
        var directBranchResult = HydraulicBranchCalculator.Calculate(
            branch,
            Water,
            flow,
            dynamicViscosityPascalSeconds: 0.001);

        var branchResult = result.BranchResults.Single();
        Assert.Same(branch, branchResult.Branch);
        Assert.Equal(directBranchResult.LocalPressureLossPascals, branchResult.BranchResult.LocalPressureLossPascals);
        Assert.Equal(2000, result.RequiredPumpPressureIncreasePascals, precision: 6);
    }

    [Fact]
    public void Calculate_EvaluatesMultipleBranchesAndSelectsCriticalBranch()
    {
        var lowLossBranch = CreateBranch("branch-low", zeta: 1);
        var highLossBranch = CreateBranch("branch-high", zeta: 3);
        var flow = CreateFlowForVelocity(lowLossBranch, velocityMetersPerSecond: 2);
        var network = new HydraulicNetwork(
            "network-1",
            "Network",
            new[]
            {
                new HydraulicBranchFlow(lowLossBranch, flow),
                new HydraulicBranchFlow(highLossBranch, flow)
            });

        var result = HydraulicNetworkCalculator.Calculate(
            network,
            Water,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(2, result.BranchResults.Count);
        Assert.Equal("branch-high", result.CriticalBranchResult.BranchId);
        Assert.Equal(6000, result.RequiredPumpPressureIncreasePascals, precision: 6);
    }

    [Fact]
    public void Calculate_ComputesRequiredPumpHeadMeters()
    {
        var branch = CreateBranch("branch-1", zeta: 1);
        var flow = CreateFlowForVelocity(branch, velocityMetersPerSecond: 2);
        var network = new HydraulicNetwork(
            "network-1",
            "Network",
            new[] { new HydraulicBranchFlow(branch, flow) });

        var result = HydraulicNetworkCalculator.Calculate(
            network,
            Water,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Equal(2000 / (1000 * 9.80665), result.RequiredPumpHeadMeters!.Value, precision: 12);
    }

    [Fact]
    public void Calculate_LeavesRequiredPumpHeadNullWhenDensityIsZero()
    {
        var zeroDensityFluid = new Fluid("zero", "Zero density", densityKilogramsPerCubicMeter: 0);
        var branch = CreateBranch("branch-1", zeta: 1);
        var flow = CreateFlowForVelocity(branch, velocityMetersPerSecond: 2);
        var network = new HydraulicNetwork(
            "network-1",
            "Network",
            new[] { new HydraulicBranchFlow(branch, flow) });

        var result = HydraulicNetworkCalculator.Calculate(
            network,
            zeroDensityFluid,
            dynamicViscosityPascalSeconds: 0.001);

        Assert.Null(result.RequiredPumpHeadMeters);
    }

    [Fact]
    public void Calculate_KeepsBranchNetBalanceFromExistingPump()
    {
        var branch = CreateBranch(
            "branch-1",
            zeta: 1,
            pump: new Pump(
                "pump-1",
                "Pump",
                new PumpCurve(new[]
                {
                    new PumpCurvePoint(0, 1),
                    new PumpCurvePoint(0.02, 1)
                })));
        var flow = CreateFlowForVelocity(branch, velocityMetersPerSecond: 2);
        var network = new HydraulicNetwork(
            "network-1",
            "Network",
            new[] { new HydraulicBranchFlow(branch, flow) });

        var result = HydraulicNetworkCalculator.Calculate(
            network,
            Water,
            dynamicViscosityPascalSeconds: 0.001);

        var branchResult = result.BranchResults.Single().BranchResult;
        Assert.Equal(1000 * 9.80665, branchResult.PumpPressureIncreasePascals, precision: 6);
        Assert.Equal(2000, result.RequiredPumpPressureIncreasePascals, precision: 6);
        Assert.Equal(1000 * 9.80665 - 2000, branchResult.NetPressureBalancePascals, precision: 6);
    }

    [Fact]
    public void Calculate_RejectsNonPositiveDynamicViscosity()
    {
        var branch = CreateBranch("branch-1", zeta: 1);
        var network = new HydraulicNetwork(
            "network-1",
            "Network",
            new[] { new HydraulicBranchFlow(branch, volumetricFlowRateCubicMetersPerSecond: 0.001) });

        Assert.Throws<ArgumentOutOfRangeException>(
            () => HydraulicNetworkCalculator.Calculate(
                network,
                Water,
                dynamicViscosityPascalSeconds: 0));
    }

    private static HydraulicBranch CreateBranch(string id, double zeta, Pump? pump = null)
    {
        return new HydraulicBranch(
            id,
            id,
            new[] { new Pipe($"{id}-pipe", lengthMeters: 0, innerDiameterMeters: 0.1) },
            localResistances: new[] { new LocalResistance($"{id}-res", $"{id} resistance", zeta) },
            pump: pump);
    }

    private static double CreateFlowForVelocity(HydraulicBranch branch, double velocityMetersPerSecond)
    {
        return PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(branch.LocalResistanceReferencePipe)
            * velocityMetersPerSecond;
    }
}
