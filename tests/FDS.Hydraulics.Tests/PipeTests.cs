using FDS.Core.Models;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class PipeTests
{
    [Fact]
    public void Constructor_StoresPipeGeometryInMeters()
    {
        var pipe = new Pipe(
            "pipe-1",
            lengthMeters: 12.5,
            innerDiameterMeters: 0.05,
            fromNodeId: "node-a",
            toNodeId: "node-b",
            roughnessMeters: 0.0001);

        Assert.Equal("pipe-1", pipe.Id);
        Assert.Equal("node-a", pipe.FromNodeId);
        Assert.Equal("node-b", pipe.ToNodeId);
        Assert.Equal(12.5, pipe.LengthMeters);
        Assert.Equal(0.05, pipe.InnerDiameterMeters);
        Assert.Equal(0.0001, pipe.RoughnessMeters);
    }

    [Fact]
    public void FromEdge_MapsCoreEdgeToPipe()
    {
        var edge = new Edge("edge-1", "node-a", "node-b", lengthMeters: 3, diameterMeters: 0.04);

        var pipe = Pipe.FromEdge(edge, roughnessMeters: 0.00005);

        Assert.Equal("edge-1", pipe.Id);
        Assert.Equal("node-a", pipe.FromNodeId);
        Assert.Equal("node-b", pipe.ToNodeId);
        Assert.Equal(3, pipe.LengthMeters);
        Assert.Equal(0.04, pipe.InnerDiameterMeters);
        Assert.Equal(0.00005, pipe.RoughnessMeters);
    }

    [Fact]
    public void Constructor_RejectsNegativeLength()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Pipe("pipe-1", lengthMeters: -0.01, innerDiameterMeters: 0.05));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Constructor_RejectsNonPositiveInnerDiameter(double innerDiameterMeters)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Pipe("pipe-1", lengthMeters: 1, innerDiameterMeters: innerDiameterMeters));
    }

    [Fact]
    public void Constructor_RejectsNegativeRoughness()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Pipe("pipe-1", lengthMeters: 1, innerDiameterMeters: 0.05, roughnessMeters: -0.0001));
    }

    [Fact]
    public void Constructor_RejectsEmptyId()
    {
        Assert.Throws<ArgumentException>(() => new Pipe(" ", lengthMeters: 1, innerDiameterMeters: 0.05));
    }
}
