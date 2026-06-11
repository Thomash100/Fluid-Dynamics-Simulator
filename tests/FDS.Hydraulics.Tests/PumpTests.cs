using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class PumpTests
{
    [Fact]
    public void Constructor_StoresPumpData()
    {
        var curve = CreateCurve();
        var efficiencyCurve = CreateEfficiencyCurve();

        var pump = new Pump("pump-1", "Primary pump", curve, efficiencyCurve);

        Assert.Equal("pump-1", pump.Id);
        Assert.Equal("Primary pump", pump.Name);
        Assert.Same(curve, pump.Curve);
        Assert.Same(efficiencyCurve, pump.EfficiencyCurve);
    }

    [Fact]
    public void Constructor_RejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(() => new Pump("pump-1", " ", CreateCurve()));
    }

    [Fact]
    public void Constructor_RejectsNullCurve()
    {
        Assert.Throws<ArgumentNullException>(() => new Pump("pump-1", "Primary pump", curve: null!));
    }

    private static PumpCurve CreateCurve()
    {
        return new PumpCurve(new[]
        {
            new PumpCurvePoint(0, 30),
            new PumpCurvePoint(0.02, 20)
        });
    }

    private static PumpEfficiencyCurve CreateEfficiencyCurve()
    {
        return new PumpEfficiencyCurve(new[]
        {
            new PumpEfficiencyPoint(0, 0.4),
            new PumpEfficiencyPoint(0.02, 0.8)
        });
    }
}
