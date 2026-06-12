using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class SmallHydraulicNetworkSolverTests
{
    private static readonly Fluid Water = new("water", "Water", densityKilogramsPerCubicMeter: 1000);

    [Fact]
    public void Solve_ConvergesSingleBranchNetworkFromSourceAndSinkFlows()
    {
        var branch = CreateBranch("branch-1", "source", "sink", zeta: 1);
        var expectedFlow = CreateFlowForVelocity(branch, velocityMetersPerSecond: 2);
        var input = CreateInput(
            new[] { branch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", expectedFlow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", expectedFlow)
            });

        var result = new SmallHydraulicNetworkSolver().Solve(input);

        Assert.Equal(HydraulicSolverStatus.Converged, result.Status);
        Assert.Equal(expectedFlow, result.SolvedVolumetricFlowRatesCubicMetersPerSecond["branch-1"], precision: 12);
        Assert.Equal(0, result.MaxNodeBalanceResidualCubicMetersPerSecond, precision: 12);
        Assert.Equal(0, result.MaxPressureResidualPascals);
        Assert.Empty(result.PressureResiduals);
        Assert.NotEmpty(result.IterationHistory);
    }

    [Fact]
    public void Solve_ConvergesTwoParallelBranchesWithKnownTotalFlowAndPressureDifference()
    {
        var lowResistanceBranch = CreateBranch("branch-low", "source", "sink", zeta: 1);
        var highResistanceBranch = CreateBranch("branch-high", "source", "sink", zeta: 4);
        var lowResistanceFlow = CreateFlowForVelocity(lowResistanceBranch, velocityMetersPerSecond: 2);
        var highResistanceFlow = CreateFlowForVelocity(highResistanceBranch, velocityMetersPerSecond: 1);
        var totalFlow = lowResistanceFlow + highResistanceFlow;
        var input = CreateInput(
            new[] { lowResistanceBranch, highResistanceBranch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", totalFlow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", totalFlow),
                HydraulicBoundaryCondition.KnownPressureDifference("dp", "source", "sink", 2000)
            });

        var result = new SmallHydraulicNetworkSolver().Solve(input);

        Assert.Equal(HydraulicSolverStatus.Converged, result.Status);
        Assert.Equal(lowResistanceFlow, result.SolvedVolumetricFlowRatesCubicMetersPerSecond["branch-low"], precision: 9);
        Assert.Equal(highResistanceFlow, result.SolvedVolumetricFlowRatesCubicMetersPerSecond["branch-high"], precision: 9);
        Assert.Equal(0, result.MaxNodeBalanceResidualCubicMetersPerSecond, precision: 9);
        Assert.Equal(0, result.MaxPressureResidualPascals, precision: 6);
        Assert.Equal(2, result.PressureResiduals.Count);
    }

    [Fact]
    public void Solve_UsesKnownPressureDifferenceAsPressureResidualTarget()
    {
        var branch = CreateBranch("branch-1", "source", "sink", zeta: 1);
        var expectedFlow = CreateFlowForVelocity(branch, velocityMetersPerSecond: 2);
        var input = CreateInput(
            new[] { branch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", expectedFlow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", expectedFlow),
                HydraulicBoundaryCondition.KnownPressureDifference("dp", "source", "sink", 2000)
            });

        var result = new SmallHydraulicNetworkSolver().Solve(input);

        var pressureResidual = result.PressureResiduals.Single();
        Assert.Equal(HydraulicSolverStatus.Converged, result.Status);
        Assert.Equal(2000, pressureResidual.AvailablePressureIncreasePascals, precision: 6);
        Assert.Equal(2000, pressureResidual.RequiredPressureIncreasePascals, precision: 6);
        Assert.Equal(0, pressureResidual.ResidualPressurePascals, precision: 6);
    }

    [Fact]
    public void Solve_UsesPumpBoundaryAsFixedPressureIncrease()
    {
        var branch = CreateBranch("branch-1", "source", "sink", zeta: 1);
        var expectedFlow = CreateFlowForVelocity(branch, velocityMetersPerSecond: 2);
        var fixedHeadMeters = 2000 / (1000 * PumpCalculator.StandardGravityMetersPerSecondSquared);
        var pump = new Pump(
            "pump-1",
            "Fixed head pump",
            new PumpCurve(new[]
            {
                new PumpCurvePoint(0, fixedHeadMeters),
                new PumpCurvePoint(0.1, fixedHeadMeters)
            }));
        var input = CreateInput(
            new[] { branch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", expectedFlow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", expectedFlow),
                HydraulicBoundaryCondition.PumpCurve("pump-boundary", "source", "sink", pump)
            });

        var result = new SmallHydraulicNetworkSolver().Solve(input);

        Assert.Equal(HydraulicSolverStatus.Converged, result.Status);
        Assert.Equal(expectedFlow, result.SolvedVolumetricFlowRatesCubicMetersPerSecond["branch-1"], precision: 9);
        Assert.Equal(0, result.MaxPressureResidualPascals, precision: 6);
    }

    [Fact]
    public void Solve_ReturnsMaxIterationsReachedWhenResidualsRemainAboveTolerance()
    {
        var branch = CreateBranch("branch-1", "source", "sink", zeta: 1);
        var expectedFlow = CreateFlowForVelocity(branch, velocityMetersPerSecond: 2);
        var input = CreateInput(
            new[] { branch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", expectedFlow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", expectedFlow),
                HydraulicBoundaryCondition.KnownPressureDifference("dp", "source", "sink", 2000)
            },
            new HydraulicSolverOptions(
                maxIterations: 1,
                flowResidualToleranceCubicMetersPerSecond: 1e-12,
                pressureResidualTolerancePascals: 1e-9,
                relaxationFactor: 0.1));

        var result = new SmallHydraulicNetworkSolver().Solve(input);

        Assert.Equal(HydraulicSolverStatus.MaxIterationsReached, result.Status);
        Assert.Equal(1, result.Iterations);
        Assert.True(result.MaxNodeBalanceResidualCubicMetersPerSecond > 0);
        Assert.True(result.MaxPressureResidualPascals > 0);
    }

    [Fact]
    public void Solve_ReturnsInvalidInputForBranchWithoutTopologyReferences()
    {
        var branch = new HydraulicBranch(
            "branch-1",
            "Invalid branch",
            new[] { new Pipe("pipe-1", lengthMeters: 0, innerDiameterMeters: 0.1) },
            localResistances: new[] { new LocalResistance("res-1", "Resistance", zeta: 1) });
        var input = CreateInput(
            new[] { branch },
            Array.Empty<HydraulicBoundaryCondition>());

        var result = new SmallHydraulicNetworkSolver().Solve(input);

        Assert.Equal(HydraulicSolverStatus.InvalidInput, result.Status);
        Assert.Equal(0, result.Iterations);
        Assert.Empty(result.IterationHistory);
    }

    private static HydraulicSolverInput CreateInput(
        IEnumerable<HydraulicBranch> branches,
        IEnumerable<HydraulicBoundaryCondition> boundaryConditions,
        HydraulicSolverOptions? options = null)
    {
        var topology = new Network(
            "network-1",
            new[]
            {
                new Node("source"),
                new Node("sink")
            },
            Array.Empty<Edge>(),
            Water);

        return new HydraulicSolverInput(
            topology,
            branches,
            Water,
            dynamicViscosityPascalSeconds: 0.001,
            boundaryConditions,
            options);
    }

    private static HydraulicBranch CreateBranch(string id, string fromNodeId, string toNodeId, double zeta)
    {
        return new HydraulicBranch(
            id,
            id,
            new[]
            {
                new Pipe(
                    $"{id}-pipe",
                    lengthMeters: 0,
                    innerDiameterMeters: 0.1,
                    fromNodeId,
                    toNodeId)
            },
            localResistances: new[] { new LocalResistance($"{id}-res", $"{id} resistance", zeta) });
    }

    private static double CreateFlowForVelocity(HydraulicBranch branch, double velocityMetersPerSecond)
    {
        return PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(branch.LocalResistanceReferencePipe)
            * velocityMetersPerSecond;
    }
}
