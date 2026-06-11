using FDS.Core.Models;
using FDS.Hydraulics.Internal;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Calculations;

/// <summary>
/// Evaluates multiple hydraulic branches with known flow rates. It does not
/// solve flow distribution, operating points, or pump selection.
/// </summary>
public static class HydraulicNetworkCalculator
{
    public static HydraulicNetworkResult Calculate(
        HydraulicNetwork network,
        Fluid fluid,
        double dynamicViscosityPascalSeconds,
        double gravitationalAccelerationMetersPerSecondSquared = PumpCalculator.StandardGravityMetersPerSecondSquared)
    {
        ArgumentNullException.ThrowIfNull(network);
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsurePositiveFinite(
            dynamicViscosityPascalSeconds,
            nameof(dynamicViscosityPascalSeconds));
        HydraulicValidation.EnsurePositiveFinite(
            gravitationalAccelerationMetersPerSecondSquared,
            nameof(gravitationalAccelerationMetersPerSecondSquared));

        var branchResults = network.BranchFlows
            .Select(branchFlow => CalculateBranchResult(
                branchFlow,
                fluid,
                dynamicViscosityPascalSeconds,
                gravitationalAccelerationMetersPerSecondSquared))
            .ToList();

        return new HydraulicNetworkResult(branchResults);
    }

    private static HydraulicNetworkBranchResult CalculateBranchResult(
        HydraulicBranchFlow branchFlow,
        Fluid fluid,
        double dynamicViscosityPascalSeconds,
        double gravitationalAccelerationMetersPerSecondSquared)
    {
        var branchResult = HydraulicBranchCalculator.Calculate(
            branchFlow.Branch,
            fluid,
            branchFlow.VolumetricFlowRateCubicMetersPerSecond,
            dynamicViscosityPascalSeconds,
            gravitationalAccelerationMetersPerSecondSquared);
        var requiredPumpPressureIncreasePascals =
            branchResult.PipePressureLossPascals + branchResult.LocalPressureLossPascals;

        return new HydraulicNetworkBranchResult(
            branchFlow.Branch,
            branchResult,
            requiredPumpPressureIncreasePascals,
            CalculateRequiredPumpHeadMeters(
                requiredPumpPressureIncreasePascals,
                fluid,
                gravitationalAccelerationMetersPerSecondSquared));
    }

    private static double? CalculateRequiredPumpHeadMeters(
        double requiredPumpPressureIncreasePascals,
        Fluid fluid,
        double gravitationalAccelerationMetersPerSecondSquared)
    {
        if (fluid.DensityKilogramsPerCubicMeter == 0)
        {
            return null;
        }

        return requiredPumpPressureIncreasePascals
            / (fluid.DensityKilogramsPerCubicMeter * gravitationalAccelerationMetersPerSecondSquared);
    }
}
