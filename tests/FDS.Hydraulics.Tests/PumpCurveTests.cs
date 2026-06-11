using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class PumpCurveTests
{
    [Fact]
    public void Constructor_SortsSupportPointsByFlow()
    {
        var curve = new PumpCurve(new[]
        {
            new PumpCurvePoint(0.02, 12),
            new PumpCurvePoint(0, 30),
            new PumpCurvePoint(0.01, 24)
        });

        Assert.Equal(0, curve.Points[0].VolumetricFlowRateCubicMetersPerSecond);
        Assert.Equal(0.01, curve.Points[1].VolumetricFlowRateCubicMetersPerSecond);
        Assert.Equal(0.02, curve.Points[2].VolumetricFlowRateCubicMetersPerSecond);
    }

    [Fact]
    public void InterpolateHeadMeters_LinearlyInterpolatesBetweenSupportPoints()
    {
        var curve = CreateDefaultCurve();

        var head = curve.InterpolateHeadMeters(0.005);

        Assert.Equal(27.5, head, precision: 12);
    }

    [Fact]
    public void InterpolateHeadMeters_ReturnsBoundaryValues()
    {
        var curve = CreateDefaultCurve();

        Assert.Equal(30, curve.InterpolateHeadMeters(0), precision: 12);
        Assert.Equal(15, curve.InterpolateHeadMeters(0.02), precision: 12);
    }

    [Fact]
    public void InterpolateHeadMeters_RejectsFlowOutsideCurveRange()
    {
        var curve = CreateDefaultCurve();

        Assert.Throws<ArgumentOutOfRangeException>(() => curve.InterpolateHeadMeters(0.021));
    }

    [Fact]
    public void Constructor_RejectsDuplicateFlowRates()
    {
        Assert.Throws<ArgumentException>(
            () => new PumpCurve(new[]
            {
                new PumpCurvePoint(0, 30),
                new PumpCurvePoint(0, 25)
            }));
    }

    [Fact]
    public void Constructor_RejectsNullSupportPoint()
    {
        Assert.Throws<ArgumentException>(
            () => new PumpCurve(new PumpCurvePoint[]
            {
                new(0, 30),
                null!
            }));
    }

    [Fact]
    public void Constructor_RejectsIncreasingHeadWithIncreasingFlow()
    {
        Assert.Throws<ArgumentException>(
            () => new PumpCurve(new[]
            {
                new PumpCurvePoint(0, 20),
                new PumpCurvePoint(0.01, 25)
            }));
    }

    [Fact]
    public void PumpCurvePoint_RejectsNegativeHead()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PumpCurvePoint(0, -1));
    }

    private static PumpCurve CreateDefaultCurve()
    {
        return new PumpCurve(new[]
        {
            new PumpCurvePoint(0, 30),
            new PumpCurvePoint(0.01, 25),
            new PumpCurvePoint(0.02, 15)
        });
    }
}
