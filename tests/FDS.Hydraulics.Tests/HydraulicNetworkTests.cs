using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class HydraulicNetworkTests
{
    [Fact]
    public void Constructor_StoresBranchFlows()
    {
        var branch = CreateBranch("branch-1");
        var flow = new HydraulicBranchFlow(branch, volumetricFlowRateCubicMetersPerSecond: 0.001);

        var network = new HydraulicNetwork("network-1", "Network", new[] { flow });

        Assert.Equal("network-1", network.Id);
        Assert.Equal("Network", network.Name);
        Assert.Same(flow, network.BranchFlows.Single());
    }

    [Fact]
    public void Constructor_RejectsEmptyBranchFlows()
    {
        Assert.Throws<ArgumentException>(
            () => new HydraulicNetwork("network-1", "Network", Array.Empty<HydraulicBranchFlow>()));
    }

    [Fact]
    public void Constructor_RejectsDuplicateBranchIds()
    {
        var first = new HydraulicBranchFlow(CreateBranch("branch-1"), volumetricFlowRateCubicMetersPerSecond: 0.001);
        var second = new HydraulicBranchFlow(CreateBranch("branch-1"), volumetricFlowRateCubicMetersPerSecond: 0.002);

        Assert.Throws<ArgumentException>(
            () => new HydraulicNetwork("network-1", "Network", new[] { first, second }));
    }

    [Fact]
    public void Constructor_RejectsNullBranchFlow()
    {
        Assert.Throws<ArgumentException>(
            () => new HydraulicNetwork(
                "network-1",
                "Network",
                new HydraulicBranchFlow[] { null! }));
    }

    [Fact]
    public void HydraulicBranchFlow_RejectsNegativeFlow()
    {
        var branch = CreateBranch("branch-1");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => new HydraulicBranchFlow(branch, volumetricFlowRateCubicMetersPerSecond: -0.001));
    }

    private static HydraulicBranch CreateBranch(string id)
    {
        return new HydraulicBranch(
            id,
            id,
            new[] { new Pipe($"{id}-pipe", lengthMeters: 0, innerDiameterMeters: 0.1) });
    }
}
