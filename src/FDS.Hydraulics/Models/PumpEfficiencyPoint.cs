using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Pump efficiency support point. Volumetric flow is stored in m^3/s and
/// efficiency is stored as a fraction from 0 to 1.
/// </summary>
public sealed class PumpEfficiencyPoint
{
    public PumpEfficiencyPoint(double volumetricFlowRateCubicMetersPerSecond, double efficiency)
    {
        VolumetricFlowRateCubicMetersPerSecond = HydraulicValidation.EnsureNonNegativeFinite(
            volumetricFlowRateCubicMetersPerSecond,
            nameof(volumetricFlowRateCubicMetersPerSecond));
        Efficiency = HydraulicValidation.EnsurePositiveFraction(efficiency, nameof(efficiency));
    }

    public double VolumetricFlowRateCubicMetersPerSecond { get; }

    public double Efficiency { get; }
}
