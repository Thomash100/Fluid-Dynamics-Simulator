using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class HydraulicBranchTests
{
    [Fact]
    public void Constructor_StoresBranchComponents()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1);
        var resistance = new LocalResistance("res-1", "Resistance", zeta: 1);
        var fitting = new Fitting("fit-1", "Bend", zeta: 2, FittingKind.Bend);
        var valve = new Valve("valve-1", "Valve", new LocalResistance("valve-zeta", "Valve zeta", zeta: 3));
        var pump = CreatePump();

        var branch = new HydraulicBranch(
            "branch-1",
            "Primary branch",
            new[] { pipe },
            new[] { resistance },
            new[] { fitting },
            new[] { valve },
            pump);

        Assert.Equal("branch-1", branch.Id);
        Assert.Equal("Primary branch", branch.Name);
        Assert.Same(pipe, branch.Pipes.Single());
        Assert.Same(resistance, branch.LocalResistances.Single());
        Assert.Same(fitting, branch.Fittings.Single());
        Assert.Same(valve, branch.Valves.Single());
        Assert.Same(pump, branch.Pump);
        Assert.Same(pipe, branch.LocalResistanceReferencePipe);
    }

    [Fact]
    public void Constructor_RejectsEmptyPipeList()
    {
        Assert.Throws<ArgumentException>(
            () => new HydraulicBranch("branch-1", "Empty branch", Array.Empty<Pipe>()));
    }

    [Fact]
    public void Constructor_RejectsNullComponent()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1);

        Assert.Throws<ArgumentException>(
            () => new HydraulicBranch(
                "branch-1",
                "Invalid branch",
                new[] { pipe },
                localResistances: new LocalResistance[] { null! }));
    }

    [Fact]
    public void Constructor_RejectsReferencePipeOutsideBranch()
    {
        var pipe = new Pipe("pipe-1", lengthMeters: 10, innerDiameterMeters: 0.1);
        var outsidePipe = new Pipe("pipe-2", lengthMeters: 10, innerDiameterMeters: 0.1);

        Assert.Throws<ArgumentException>(
            () => new HydraulicBranch(
                "branch-1",
                "Invalid branch",
                new[] { pipe },
                localResistanceReferencePipe: outsidePipe));
    }

    private static Pump CreatePump()
    {
        return new Pump(
            "pump-1",
            "Pump",
            new PumpCurve(new[]
            {
                new PumpCurvePoint(0, 10),
                new PumpCurvePoint(0.02, 8)
            }));
    }
}
