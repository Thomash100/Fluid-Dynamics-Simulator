using FDS.Core.Models;
using FDS.Hydraulics.Internal;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Calculations;

/// <summary>
/// Single-pipe hydraulic helper calculations. These methods do not solve a
/// network and do not model pumps or valves.
/// </summary>
public static class PipeFlowCalculator
{
    private const double LaminarReynoldsLimit = 2300;

    /// <summary>
    /// Calculates pipe cross-sectional area in m^2.
    /// </summary>
    public static double CalculateCrossSectionalAreaSquareMeters(Pipe pipe)
    {
        ArgumentNullException.ThrowIfNull(pipe);

        var radiusMeters = pipe.InnerDiameterMeters / 2;
        return Math.PI * radiusMeters * radiusMeters;
    }

    /// <summary>
    /// Calculates signed flow velocity in m/s from volumetric flow rate in m^3/s.
    /// A negative result indicates flow opposite to the pipe direction.
    /// </summary>
    public static double CalculateVelocityMetersPerSecond(Pipe pipe, double volumetricFlowRateCubicMetersPerSecond)
    {
        HydraulicValidation.EnsureFinite(volumetricFlowRateCubicMetersPerSecond, nameof(volumetricFlowRateCubicMetersPerSecond));

        return volumetricFlowRateCubicMetersPerSecond / CalculateCrossSectionalAreaSquareMeters(pipe);
    }

    /// <summary>
    /// Calculates Reynolds number using density in kg/m^3, velocity in m/s,
    /// inner diameter in m, and dynamic viscosity in Pa*s.
    /// </summary>
    public static double CalculateReynoldsNumber(
        Pipe pipe,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond,
        double dynamicViscosityPascalSeconds)
    {
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsurePositiveFinite(dynamicViscosityPascalSeconds, nameof(dynamicViscosityPascalSeconds));

        var velocityMetersPerSecond = CalculateVelocityMetersPerSecond(pipe, volumetricFlowRateCubicMetersPerSecond);

        return fluid.DensityKilogramsPerCubicMeter
            * Math.Abs(velocityMetersPerSecond)
            * pipe.InnerDiameterMeters
            / dynamicViscosityPascalSeconds;
    }

    /// <summary>
    /// Estimates the Darcy friction factor. Laminar flow uses 64/Re. Non-laminar
    /// flow currently uses the simple Blasius approximation for smooth pipes.
    /// </summary>
    public static double EstimateDarcyFrictionFactor(double reynoldsNumber)
    {
        HydraulicValidation.EnsurePositiveFinite(reynoldsNumber, nameof(reynoldsNumber));

        if (reynoldsNumber < LaminarReynoldsLimit)
        {
            return 64 / reynoldsNumber;
        }

        return 0.3164 / Math.Pow(reynoldsNumber, 0.25);
    }

    /// <summary>
    /// Calculates a prepared Darcy-Weisbach pipe pressure loss in Pa for a
    /// single pipe and known flow. The returned value is a positive loss
    /// magnitude, not a network pressure solution.
    /// </summary>
    public static double CalculateDarcyWeisbachPressureLossPascals(
        Pipe pipe,
        Fluid fluid,
        double volumetricFlowRateCubicMetersPerSecond,
        double dynamicViscosityPascalSeconds)
    {
        ArgumentNullException.ThrowIfNull(pipe);
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsurePositiveFinite(dynamicViscosityPascalSeconds, nameof(dynamicViscosityPascalSeconds));

        var velocityMetersPerSecond = CalculateVelocityMetersPerSecond(pipe, volumetricFlowRateCubicMetersPerSecond);
        var speedMetersPerSecond = Math.Abs(velocityMetersPerSecond);

        if (speedMetersPerSecond == 0 || pipe.LengthMeters == 0 || fluid.DensityKilogramsPerCubicMeter == 0)
        {
            return 0;
        }

        var reynoldsNumber = CalculateReynoldsNumber(
            pipe,
            fluid,
            volumetricFlowRateCubicMetersPerSecond,
            dynamicViscosityPascalSeconds);
        var frictionFactor = EstimateDarcyFrictionFactor(reynoldsNumber);

        return frictionFactor
            * (pipe.LengthMeters / pipe.InnerDiameterMeters)
            * (fluid.DensityKilogramsPerCubicMeter * speedMetersPerSecond * speedMetersPerSecond / 2);
    }
}
