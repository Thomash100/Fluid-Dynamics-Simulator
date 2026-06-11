using FDS.Core.Models;
using FDS.Hydraulics.Internal;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Calculations;

/// <summary>
/// Single-pump helper calculations. These methods evaluate a known pump curve
/// at a supplied flow rate and do not find an operating point.
/// </summary>
public static class PumpCalculator
{
    public const double StandardGravityMetersPerSecondSquared = 9.80665;

    public static double InterpolateHeadMeters(Pump pump, double volumetricFlowRateCubicMetersPerSecond)
    {
        ArgumentNullException.ThrowIfNull(pump);

        return pump.Curve.InterpolateHeadMeters(volumetricFlowRateCubicMetersPerSecond);
    }

    public static double CalculateHydraulicPowerWatts(
        Pump pump,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond,
        double gravitationalAccelerationMetersPerSecondSquared = StandardGravityMetersPerSecondSquared)
    {
        ArgumentNullException.ThrowIfNull(pump);
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsurePositiveFinite(
            gravitationalAccelerationMetersPerSecondSquared,
            nameof(gravitationalAccelerationMetersPerSecondSquared));

        var headMeters = pump.Curve.InterpolateHeadMeters(volumetricFlowRateCubicMetersPerSecond);

        return fluid.DensityKilogramsPerCubicMeter
            * gravitationalAccelerationMetersPerSecondSquared
            * volumetricFlowRateCubicMetersPerSecond
            * headMeters;
    }

    public static double CalculatePressureIncreasePascals(
        Pump pump,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond,
        double gravitationalAccelerationMetersPerSecondSquared = StandardGravityMetersPerSecondSquared)
    {
        ArgumentNullException.ThrowIfNull(pump);
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsurePositiveFinite(
            gravitationalAccelerationMetersPerSecondSquared,
            nameof(gravitationalAccelerationMetersPerSecondSquared));

        var headMeters = pump.Curve.InterpolateHeadMeters(volumetricFlowRateCubicMetersPerSecond);

        return fluid.DensityKilogramsPerCubicMeter
            * gravitationalAccelerationMetersPerSecondSquared
            * headMeters;
    }

    public static double CalculateShaftPowerWatts(
        Pump pump,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond,
        double gravitationalAccelerationMetersPerSecondSquared = StandardGravityMetersPerSecondSquared)
    {
        ArgumentNullException.ThrowIfNull(pump);

        if (pump.EfficiencyCurve is null)
        {
            throw new InvalidOperationException("Pump has no efficiency curve.");
        }

        var hydraulicPowerWatts = CalculateHydraulicPowerWatts(
            pump,
            fluid,
            volumetricFlowRateCubicMetersPerSecond,
            gravitationalAccelerationMetersPerSecondSquared);
        var efficiency = pump.EfficiencyCurve.InterpolateEfficiency(volumetricFlowRateCubicMetersPerSecond);

        return hydraulicPowerWatts / efficiency;
    }
}
