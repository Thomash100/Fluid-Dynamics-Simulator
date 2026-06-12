using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class HydraulicSolverPreparationCalculatorTests
{
    private static readonly Fluid Water = new("water", "Water", densityKilogramsPerCubicMeter: 1000);

    [Fact]
    public void Prepare_CalculatesBalancedNodeResidualsForReferenceNetwork()
    {
        var topology = new Network(
            "network-1",
            new[]
            {
                new Node("source"),
                new Node("sink")
            },
            new[]
            {
                new Edge(
                    "edge-1",
                    "source",
                    "sink",
                    lengthMeters: 10,
                    diameterMeters: 0.1,
                    volumetricFlowRateCubicMetersPerSecond: 0.01)
            });
        var boundaries = new[]
        {
            HydraulicBoundaryCondition.SourceFlow("source-flow", "source", 0.01),
            HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", 0.01)
        };

        var result = HydraulicSolverPreparationCalculator.Prepare(topology, boundaries);

        Assert.Equal(HydraulicSolverStatus.Prepared, result.Status);
        Assert.Equal(0, result.Iterations);
        Assert.All(result.NodeBalances, balance => Assert.Equal(0, balance.ResidualFlowCubicMetersPerSecond, precision: 12));
        Assert.Equal(0, result.MaxNodeBalanceResidualCubicMetersPerSecond);
    }

    [Fact]
    public void Prepare_ReportsNodeBalanceResidualForUnbalancedReferenceNetwork()
    {
        var topology = new Network(
            "network-1",
            new[]
            {
                new Node("source"),
                new Node("junction"),
                new Node("sink")
            },
            new[]
            {
                new Edge(
                    "edge-1",
                    "source",
                    "junction",
                    lengthMeters: 10,
                    diameterMeters: 0.1,
                    volumetricFlowRateCubicMetersPerSecond: 0.1),
                new Edge(
                    "edge-2",
                    "junction",
                    "sink",
                    lengthMeters: 10,
                    diameterMeters: 0.1,
                    volumetricFlowRateCubicMetersPerSecond: 0.08)
            },
            Water);
        var boundaries = new[]
        {
            HydraulicBoundaryCondition.SourceFlow("source-flow", "source", 0.1),
            HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", 0.08)
        };

        var result = HydraulicSolverPreparationCalculator.Prepare(topology, boundaries);
        var junctionBalance = result.NodeBalances.Single(balance => balance.NodeId == "junction");

        Assert.Equal(0.02, junctionBalance.ResidualFlowCubicMetersPerSecond, precision: 12);
        Assert.Equal(0.02, result.MaxNodeBalanceResidualCubicMetersPerSecond, precision: 12);
    }

    [Fact]
    public void Prepare_UsesSignedEdgeFlowDirection()
    {
        var topology = new Network(
            "network-1",
            new[]
            {
                new Node("node-a"),
                new Node("node-b")
            },
            new[]
            {
                new Edge(
                    "edge-1",
                    "node-a",
                    "node-b",
                    lengthMeters: 10,
                    diameterMeters: 0.1,
                    volumetricFlowRateCubicMetersPerSecond: -0.01)
            });

        var result = HydraulicSolverPreparationCalculator.Prepare(topology);

        var nodeA = result.NodeBalances.Single(balance => balance.NodeId == "node-a");
        var nodeB = result.NodeBalances.Single(balance => balance.NodeId == "node-b");
        Assert.Equal(0.01, nodeA.IncomingFlowCubicMetersPerSecond);
        Assert.Equal(0.01, nodeB.OutgoingFlowCubicMetersPerSecond);
    }

    [Fact]
    public void Prepare_ReportsBranchPressureResidualsFromFixedHydraulicNetwork()
    {
        var topology = new Network(
            "network-1",
            new[]
            {
                new Node("source"),
                new Node("sink")
            },
            new[]
            {
                new Edge(
                    "edge-1",
                    "source",
                    "sink",
                    lengthMeters: 0,
                    diameterMeters: 0.1,
                    volumetricFlowRateCubicMetersPerSecond: CreateFlowForVelocity(2))
            },
            Water);
        var branch = new HydraulicBranch(
            "branch-1",
            "Branch",
            new[] { new Pipe("pipe-1", lengthMeters: 0, innerDiameterMeters: 0.1) },
            localResistances: new[] { new LocalResistance("res-1", "Resistance", zeta: 1) });
        var hydraulicNetwork = new HydraulicNetwork(
            "hydraulic-network-1",
            "Hydraulic Network",
            new[] { new HydraulicBranchFlow(branch, CreateFlowForVelocity(2)) });

        var result = HydraulicSolverPreparationCalculator.Prepare(
            topology,
            hydraulicNetwork,
            Water,
            dynamicViscosityPascalSeconds: 0.001);

        var pressureResidual = result.PressureResiduals.Single();
        Assert.Equal("branch-1", pressureResidual.ElementId);
        Assert.Equal("Branch", pressureResidual.ElementKind);
        Assert.Equal(0, pressureResidual.AvailablePressureIncreasePascals);
        Assert.Equal(2000, pressureResidual.RequiredPressureIncreasePascals, precision: 6);
        Assert.Equal(-2000, pressureResidual.ResidualPressurePascals, precision: 6);
        Assert.Equal(2000, result.MaxPressureResidualPascals, precision: 6);
    }

    [Fact]
    public void Prepare_RejectsBoundaryConditionForUnknownNode()
    {
        var topology = new Network(
            "network-1",
            new[] { new Node("source") },
            Array.Empty<Edge>());
        var boundaries = new[]
        {
            HydraulicBoundaryCondition.SourceFlow("source-flow", "missing-node", 0.01)
        };

        Assert.Throws<ArgumentException>(
            () => HydraulicSolverPreparationCalculator.Prepare(topology, boundaries));
    }

    private static double CreateFlowForVelocity(double velocityMetersPerSecond)
    {
        var pipe = new Pipe("reference", lengthMeters: 0, innerDiameterMeters: 0.1);

        return PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe) * velocityMetersPerSecond;
    }
}
