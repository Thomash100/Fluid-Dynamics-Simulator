using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class PumpEfficiencyCurveTests
{
    [Fact]
    public void InterpolateEfficiency_LinearlyInterpolatesBetweenSupportPoints()
    {
        var curve = new PumpEfficiencyCurve(new[]
        {
            new PumpEfficiencyPoint(0, 0.4),
            new PumpEfficiencyPoint(0.02, 0.8)
        });

        var efficiency = curve.InterpolateEfficiency(0.01);

        Assert.Equal(0.6, efficiency, precision: 12);
    }

    [Fact]
    public void InterpolateEfficiency_ReturnsBoundaryValues()
    {
        var curve = new PumpEfficiencyCurve(new[]
        {
            new PumpEfficiencyPoint(0, 0.4),
            new PumpEfficiencyPoint(0.02, 0.8)
        });

        Assert.Equal(0.4, curve.InterpolateEfficiency(0), precision: 12);
        Assert.Equal(0.8, curve.InterpolateEfficiency(0.02), precision: 12);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1.01)]
    public void PumpEfficiencyPoint_RejectsInvalidEfficiency(double efficiency)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new PumpEfficiencyPoint(0, efficiency));
    }

    [Fact]
    public void Constructor_RejectsDuplicateFlowRates()
    {
        Assert.Throws<ArgumentException>(
            () => new PumpEfficiencyCurve(new[]
            {
                new PumpEfficiencyPoint(0, 0.4),
                new PumpEfficiencyPoint(0, 0.5)
            }));
    }

    [Fact]
    public void Constructor_RejectsNullSupportPoint()
    {
        Assert.Throws<ArgumentException>(
            () => new PumpEfficiencyCurve(new PumpEfficiencyPoint[]
            {
                new(0, 0.4),
                null!
            }));
    }
}
