using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Pump curve support point. Volumetric flow is stored in m^3/s, head in m.
/// </summary>
public sealed class PumpCurvePoint
{
    public PumpCurvePoint(double volumetricFlowRateCubicMetersPerSecond, double headMeters)
    {
        VolumetricFlowRateCubicMetersPerSecond = HydraulicValidation.EnsureNonNegativeFinite(
            volumetricFlowRateCubicMetersPerSecond,
            nameof(volumetricFlowRateCubicMetersPerSecond));
        HeadMeters = HydraulicValidation.EnsureNonNegativeFinite(headMeters, nameof(headMeters));
    }

    public double VolumetricFlowRateCubicMetersPerSecond { get; }

    public double HeadMeters { get; }
}
