using System.Collections.ObjectModel;

using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Pump head curve with linear interpolation between support points. It does
/// not calculate an operating point.
/// </summary>
public sealed class PumpCurve
{
    public PumpCurve(IEnumerable<PumpCurvePoint> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        var sortedPoints = points
            .Select(point => point ?? throw new ArgumentException("Pump curve points cannot contain null.", nameof(points)))
            .OrderBy(point => point.VolumetricFlowRateCubicMetersPerSecond)
            .ToList();

        if (sortedPoints.Count < 2)
        {
            throw new ArgumentException("A pump curve requires at least two support points.", nameof(points));
        }

        ValidateUniqueFlowRates(sortedPoints);
        ValidateNonIncreasingHead(sortedPoints);

        Points = new ReadOnlyCollection<PumpCurvePoint>(sortedPoints);
    }

    public IReadOnlyList<PumpCurvePoint> Points { get; }

    public double MinFlowRateCubicMetersPerSecond => Points[0].VolumetricFlowRateCubicMetersPerSecond;

    public double MaxFlowRateCubicMetersPerSecond => Points[^1].VolumetricFlowRateCubicMetersPerSecond;

    public double InterpolateHeadMeters(double volumetricFlowRateCubicMetersPerSecond)
    {
        HydraulicValidation.EnsureFinite(
            volumetricFlowRateCubicMetersPerSecond,
            nameof(volumetricFlowRateCubicMetersPerSecond));

        if (volumetricFlowRateCubicMetersPerSecond < MinFlowRateCubicMetersPerSecond
            || volumetricFlowRateCubicMetersPerSecond > MaxFlowRateCubicMetersPerSecond)
        {
            throw new ArgumentOutOfRangeException(
                nameof(volumetricFlowRateCubicMetersPerSecond),
                volumetricFlowRateCubicMetersPerSecond,
                "Flow rate is outside the pump curve range.");
        }

        for (var index = 0; index < Points.Count - 1; index++)
        {
            var lower = Points[index];
            var upper = Points[index + 1];

            if (volumetricFlowRateCubicMetersPerSecond >= lower.VolumetricFlowRateCubicMetersPerSecond
                && volumetricFlowRateCubicMetersPerSecond <= upper.VolumetricFlowRateCubicMetersPerSecond)
            {
                return Interpolate(
                    lower.VolumetricFlowRateCubicMetersPerSecond,
                    lower.HeadMeters,
                    upper.VolumetricFlowRateCubicMetersPerSecond,
                    upper.HeadMeters,
                    volumetricFlowRateCubicMetersPerSecond);
            }
        }

        throw new ArgumentOutOfRangeException(
            nameof(volumetricFlowRateCubicMetersPerSecond),
            volumetricFlowRateCubicMetersPerSecond,
            "Flow rate is outside the pump curve range.");
    }

    private static double Interpolate(double lowerX, double lowerY, double upperX, double upperY, double x)
    {
        var fraction = (x - lowerX) / (upperX - lowerX);
        return lowerY + fraction * (upperY - lowerY);
    }

    private static void ValidateUniqueFlowRates(IReadOnlyList<PumpCurvePoint> points)
    {
        for (var index = 1; index < points.Count; index++)
        {
            if (points[index].VolumetricFlowRateCubicMetersPerSecond
                == points[index - 1].VolumetricFlowRateCubicMetersPerSecond)
            {
                throw new ArgumentException("Pump curve flow rates must be unique.", nameof(points));
            }
        }
    }

    private static void ValidateNonIncreasingHead(IReadOnlyList<PumpCurvePoint> points)
    {
        for (var index = 1; index < points.Count; index++)
        {
            if (points[index].HeadMeters > points[index - 1].HeadMeters)
            {
                throw new ArgumentException("Pump curve head must not increase with increasing flow.", nameof(points));
            }
        }
    }
}
