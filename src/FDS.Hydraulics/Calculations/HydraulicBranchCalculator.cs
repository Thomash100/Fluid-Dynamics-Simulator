using FDS.Core.Models;
using FDS.Hydraulics.Internal;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Calculations;

/// <summary>
/// Evaluates a simple hydraulic branch at a supplied flow rate. It aggregates
/// component losses and pump pressure increase but does not solve an operating
/// point or network.
/// </summary>
public static class HydraulicBranchCalculator
{
    public static HydraulicBranchResult Calculate(
        HydraulicBranch branch,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond,
        double dynamicViscosityPascalSeconds,
        double gravitationalAccelerationMetersPerSecondSquared = PumpCalculator.StandardGravityMetersPerSecondSquared)
    {
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsureNonNegativeFinite(
            volumetricFlowRateCubicMetersPerSecond,
            nameof(volumetricFlowRateCubicMetersPerSecond));
        HydraulicValidation.EnsurePositiveFinite(
            dynamicViscosityPascalSeconds,
            nameof(dynamicViscosityPascalSeconds));
        HydraulicValidation.EnsurePositiveFinite(
            gravitationalAccelerationMetersPerSecondSquared,
            nameof(gravitationalAccelerationMetersPerSecondSquared));

        var pipePressureLossPascals = branch.Pipes.Sum(
            pipe => PipeFlowCalculator.CalculateDarcyWeisbachPressureLossPascals(
                pipe,
                fluid,
                volumetricFlowRateCubicMetersPerSecond,
                dynamicViscosityPascalSeconds));

        var localPressureLossPascals = CalculateLocalPressureLossPascals(
            branch,
            fluid,
            volumetricFlowRateCubicMetersPerSecond);

        var pumpPressureIncreasePascals = branch.Pump is null
            ? 0
            : PumpCalculator.CalculatePressureIncreasePascals(
                branch.Pump,
                fluid,
                volumetricFlowRateCubicMetersPerSecond,
                gravitationalAccelerationMetersPerSecondSquared);

        return new HydraulicBranchResult(
            volumetricFlowRateCubicMetersPerSecond,
            pipePressureLossPascals,
            localPressureLossPascals,
            pumpPressureIncreasePascals);
    }

    private static double CalculateLocalPressureLossPascals(
        HydraulicBranch branch,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond)
    {
        var pressureLossPascals = branch.LocalResistances.Sum(
            resistance => LocalResistanceCalculator.CalculateZetaPressureLossPascals(
                resistance,
                branch.LocalResistanceReferencePipe,
                fluid,
                volumetricFlowRateCubicMetersPerSecond));

        pressureLossPascals += branch.Fittings.Sum(
            fitting => LocalResistanceCalculator.CalculateFittingPressureLossPascals(
                fitting,
                branch.LocalResistanceReferencePipe,
                fluid,
                volumetricFlowRateCubicMetersPerSecond));

        pressureLossPascals += branch.Valves.Sum(
            valve => CalculateValvePressureLossPascals(
                valve,
                branch.LocalResistanceReferencePipe,
                fluid,
                volumetricFlowRateCubicMetersPerSecond));

        return pressureLossPascals;
    }

    private static double CalculateValvePressureLossPascals(
        Valve valve,
        Pipe referencePipe,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond)
    {
        if (valve.FlowCoefficient is not null)
        {
            return LocalResistanceCalculator.CalculateValvePressureLossFromKvPascals(
                valve.FlowCoefficient,
                fluid,
                volumetricFlowRateCubicMetersPerSecond);
        }

        if (valve.Resistance is not null)
        {
            return LocalResistanceCalculator.CalculateZetaPressureLossPascals(
                valve.Resistance,
                referencePipe,
                fluid,
                volumetricFlowRateCubicMetersPerSecond);
        }

        return 0;
    }
}
