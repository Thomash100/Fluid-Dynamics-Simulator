using FDS.Core.Models;

namespace FDS.Core.Tests;

public sealed class NetworkTests
{
    [Fact]
    public void Constructor_StoresValidatedTopology()
    {
        var nodes = new[]
        {
            new Node("node-a"),
            new Node("node-b")
        };
        var edges = new[]
        {
            new Edge("edge-1", "node-a", "node-b", lengthMeters: 3, diameterMeters: 0.04)
        };
        var fluid = new Fluid("water", "Water", densityKilogramsPerCubicMeter: 998.2);

        var network = new Network("network-1", nodes, edges, fluid);

        Assert.Equal("network-1", network.Id);
        Assert.Same(fluid, network.Fluid);
        Assert.True(network.Nodes.ContainsKey("node-a"));
        Assert.True(network.Nodes.ContainsKey("node-b"));
        Assert.True(network.Edges.ContainsKey("edge-1"));
    }

    [Fact]
    public void Constructor_RejectsDuplicateNodeIds()
    {
        var nodes = new[]
        {
            new Node("node-a"),
            new Node("node-a")
        };

        Assert.Throws<ArgumentException>(() => new Network("network-1", nodes, Array.Empty<Edge>()));
    }

    [Fact]
    public void Constructor_RejectsDuplicateEdgeIds()
    {
        var nodes = new[]
        {
            new Node("node-a"),
            new Node("node-b")
        };
        var edges = new[]
        {
            new Edge("edge-1", "node-a", "node-b", lengthMeters: 3, diameterMeters: 0.04),
            new Edge("edge-1", "node-a", "node-b", lengthMeters: 4, diameterMeters: 0.04)
        };

        Assert.Throws<ArgumentException>(() => new Network("network-1", nodes, edges));
    }

    [Fact]
    public void Constructor_RejectsDuplicateIdsAcrossNodesAndEdges()
    {
        var nodes = new[]
        {
            new Node("shared-id"),
            new Node("node-b")
        };
        var edges = new[]
        {
            new Edge("shared-id", "shared-id", "node-b", lengthMeters: 3, diameterMeters: 0.04)
        };

        Assert.Throws<ArgumentException>(() => new Network("network-1", nodes, edges));
    }

    [Fact]
    public void Constructor_RejectsEdgesThatReferenceUnknownNodes()
    {
        var nodes = new[]
        {
            new Node("node-a")
        };
        var edges = new[]
        {
            new Edge("edge-1", "node-a", "missing-node", lengthMeters: 3, diameterMeters: 0.04)
        };

        Assert.Throws<ArgumentException>(() => new Network("network-1", nodes, edges));
    }
}
