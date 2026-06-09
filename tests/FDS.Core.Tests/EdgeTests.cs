using FDS.Core.Models;

namespace FDS.Core.Tests;

public sealed class EdgeTests
{
    [Fact]
    public void Constructor_StoresTopologyAndFlowUnits()
    {
        var edge = new Edge(
            "edge-1",
            "node-a",
            "node-b",
            lengthMeters: 12.5,
            diameterMeters: 0.05,
            volumetricFlowRateCubicMetersPerSecond: 0.01,
            massFlowRateKilogramsPerSecond: 9.98);

        Assert.Equal("edge-1", edge.Id);
        Assert.Equal("node-a", edge.FromNodeId);
        Assert.Equal("node-b", edge.ToNodeId);
        Assert.Equal(12.5, edge.LengthMeters);
        Assert.Equal(0.05, edge.DiameterMeters);
        Assert.Equal(0.01, edge.VolumetricFlowRateCubicMetersPerSecond);
        Assert.Equal(9.98, edge.MassFlowRateKilogramsPerSecond);
    }

    [Fact]
    public void Constructor_RejectsNegativeLength()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Edge("edge-1", "node-a", "node-b", lengthMeters: -0.01, diameterMeters: 0.05));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Constructor_RejectsNonPositiveDiameter(double diameterMeters)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Edge("edge-1", "node-a", "node-b", lengthMeters: 1, diameterMeters: diameterMeters));
    }

    [Fact]
    public void Constructor_RejectsEmptyNodeIds()
    {
        Assert.Throws<ArgumentException>(
            () => new Edge("edge-1", " ", "node-b", lengthMeters: 1, diameterMeters: 0.05));
        Assert.Throws<ArgumentException>(
            () => new Edge("edge-1", "node-a", " ", lengthMeters: 1, diameterMeters: 0.05));
    }
}
