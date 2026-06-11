using System.Collections.ObjectModel;

using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Optional pump efficiency curve with linear interpolation. It is a basic
/// helper, not a pump selection or control strategy.
/// </summary>
public sealed class PumpEfficiencyCurve
{
    public PumpEfficiencyCurve(IEnumerable<PumpEfficiencyPoint> points)
    {
        ArgumentNullException.ThrowIfNull(points);

        var sortedPoints = points
            .Select(point => point ?? throw new ArgumentException("Efficiency curve points cannot contain null.", nameof(points)))
            .OrderBy(point => point.VolumetricFlowRateCubicMetersPerSecond)
            .ToList();

        if (sortedPoints.Count < 2)
        {
            throw new ArgumentException("An efficiency curve requires at least two support points.", nameof(points));
        }

        ValidateUniqueFlowRates(sortedPoints);

        Points = new ReadOnlyCollection<PumpEfficiencyPoint>(sortedPoints);
    }

    public IReadOnlyList<PumpEfficiencyPoint> Points { get; }

    public double MinFlowRateCubicMetersPerSecond => Points[0].VolumetricFlowRateCubicMetersPerSecond;

    public double MaxFlowRateCubicMetersPerSecond => Points[^1].VolumetricFlowRateCubicMetersPerSecond;

    public double InterpolateEfficiency(double volumetricFlowRateCubicMetersPerSecond)
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
                "Flow rate is outside the efficiency curve range.");
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
                    lower.Efficiency,
                    upper.VolumetricFlowRateCubicMetersPerSecond,
                    upper.Efficiency,
                    volumetricFlowRateCubicMetersPerSecond);
            }
        }

        throw new ArgumentOutOfRangeException(
            nameof(volumetricFlowRateCubicMetersPerSecond),
            volumetricFlowRateCubicMetersPerSecond,
            "Flow rate is outside the efficiency curve range.");
    }

    private static double Interpolate(double lowerX, double lowerY, double upperX, double upperY, double x)
    {
        var fraction = (x - lowerX) / (upperX - lowerX);
        return lowerY + fraction * (upperY - lowerY);
    }

    private static void ValidateUniqueFlowRates(IReadOnlyList<PumpEfficiencyPoint> points)
    {
        for (var index = 1; index < points.Count; index++)
        {
            if (points[index].VolumetricFlowRateCubicMetersPerSecond
                == points[index - 1].VolumetricFlowRateCubicMetersPerSecond)
            {
                throw new ArgumentException("Efficiency curve flow rates must be unique.", nameof(points));
            }
        }
    }
}
