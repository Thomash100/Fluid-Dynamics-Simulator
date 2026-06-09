using FDS.Core.Models;
using FDS.Hydraulics.Internal;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Calculations;

/// <summary>
/// Single-component local resistance calculations. These methods do not solve
/// a network and do not size control valves.
/// </summary>
public static class LocalResistanceCalculator
{
    private const double CubicMetersPerSecondToCubicMetersPerHour = 3600;
    private const double PascalsPerBar = 100000;
    private const double WaterReferenceDensityKilogramsPerCubicMeter = 1000;

    /// <summary>
    /// Calculates zeta-based local pressure loss in Pa from velocity in m/s.
    /// </summary>
    public static double CalculateZetaPressureLossPascals(
        LocalResistance resistance,
        Fluid fluid,
        double velocityMetersPerSecond)
    {
        ArgumentNullException.ThrowIfNull(resistance);
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsureFinite(velocityMetersPerSecond, nameof(velocityMetersPerSecond));

        var speedMetersPerSecond = Math.Abs(velocityMetersPerSecond);

        if (resistance.Zeta == 0 || speedMetersPerSecond == 0 || fluid.DensityKilogramsPerCubicMeter == 0)
        {
            return 0;
        }

        return resistance.Zeta
            * fluid.DensityKilogramsPerCubicMeter
            * speedMetersPerSecond
            * speedMetersPerSecond
            / 2;
    }

    /// <summary>
    /// Calculates zeta-based local pressure loss in Pa from volumetric flow in
    /// m^3/s and pipe diameter.
    /// </summary>
    public static double CalculateZetaPressureLossPascals(
        LocalResistance resistance,
        Pipe pipe,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond)
    {
        ArgumentNullException.ThrowIfNull(pipe);

        var velocityMetersPerSecond = PipeFlowCalculator.CalculateVelocityMetersPerSecond(
            pipe,
            volumetricFlowRateCubicMetersPerSecond);

        return CalculateZetaPressureLossPascals(resistance, fluid, velocityMetersPerSecond);
    }

    public static double CalculateFittingPressureLossPascals(
        Fitting fitting,
        Pipe pipe,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond)
    {
        ArgumentNullException.ThrowIfNull(fitting);

        return CalculateZetaPressureLossPascals(
            fitting.Resistance,
            pipe,
            fluid,
            volumetricFlowRateCubicMetersPerSecond);
    }

    /// <summary>
    /// Calculates valve pressure loss in Pa using the metric Kv convention:
    /// dp_bar = (rho / 1000) * (Q_m3h / Kv)^2.
    /// </summary>
    public static double CalculateValvePressureLossFromKvPascals(
        ValveFlowCoefficient coefficient,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond)
    {
        ArgumentNullException.ThrowIfNull(coefficient);
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsureFinite(
            volumetricFlowRateCubicMetersPerSecond,
            nameof(volumetricFlowRateCubicMetersPerSecond));

        var absoluteFlowCubicMetersPerHour = Math.Abs(volumetricFlowRateCubicMetersPerSecond)
            * CubicMetersPerSecondToCubicMetersPerHour;

        if (absoluteFlowCubicMetersPerHour == 0 || fluid.DensityKilogramsPerCubicMeter == 0)
        {
            return 0;
        }

        var densityRatio = fluid.DensityKilogramsPerCubicMeter / WaterReferenceDensityKilogramsPerCubicMeter;
        var pressureLossBar = densityRatio
            * Math.Pow(absoluteFlowCubicMetersPerHour / coefficient.KvCubicMetersPerHour, 2);

        return pressureLossBar * PascalsPerBar;
    }
}
