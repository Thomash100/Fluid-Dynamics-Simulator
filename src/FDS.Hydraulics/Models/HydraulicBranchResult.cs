using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Pressure balance for a simple branch at one supplied volumetric flow rate.
/// Positive net balance means pump pressure increase exceeds component losses.
/// </summary>
public sealed class HydraulicBranchResult
{
    public HydraulicBranchResult(
        double volumetricFlowRateCubicMetersPerSecond,
        double pipePressureLossPascals,
        double localPressureLossPascals,
        double pumpPressureIncreasePascals)
    {
        VolumetricFlowRateCubicMetersPerSecond = HydraulicValidation.EnsureNonNegativeFinite(
            volumetricFlowRateCubicMetersPerSecond,
            nameof(volumetricFlowRateCubicMetersPerSecond));
        PipePressureLossPascals = HydraulicValidation.EnsureNonNegativeFinite(
            pipePressureLossPascals,
            nameof(pipePressureLossPascals));
        LocalPressureLossPascals = HydraulicValidation.EnsureNonNegativeFinite(
            localPressureLossPascals,
            nameof(localPressureLossPascals));
        PumpPressureIncreasePascals = HydraulicValidation.EnsureNonNegativeFinite(
            pumpPressureIncreasePascals,
            nameof(pumpPressureIncreasePascals));
    }

    public double VolumetricFlowRateCubicMetersPerSecond { get; }

    public double PipePressureLossPascals { get; }

    public double LocalPressureLossPascals { get; }

    public double PumpPressureIncreasePascals { get; }

    public double NetPressureBalancePascals =>
        PumpPressureIncreasePascals - PipePressureLossPascals - LocalPressureLossPascals;
}
