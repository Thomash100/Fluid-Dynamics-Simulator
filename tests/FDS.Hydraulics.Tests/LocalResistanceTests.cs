using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class LocalResistanceTests
{
    [Fact]
    public void Constructor_StoresDimensionlessZeta()
    {
        var resistance = new LocalResistance("bend-90", "90 degree bend", zeta: 0.75);

        Assert.Equal("bend-90", resistance.Id);
        Assert.Equal("90 degree bend", resistance.Name);
        Assert.Equal(0.75, resistance.Zeta);
    }

    [Fact]
    public void Constructor_RejectsNegativeZeta()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new LocalResistance("invalid", "Invalid", zeta: -0.1));
    }

    [Fact]
    public void Fitting_StoresKindAndResistance()
    {
        var fitting = new Fitting("fit-1", "Bend", zeta: 0.9, FittingKind.Bend);

        Assert.Equal("fit-1", fitting.Id);
        Assert.Equal("Bend", fitting.Name);
        Assert.Equal(FittingKind.Bend, fitting.Kind);
        Assert.Equal(0.9, fitting.Resistance.Zeta);
    }
}
