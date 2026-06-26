using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;
using FDS.Hydraulics.Tests.ReferenceCases;

namespace FDS.Hydraulics.Tests;

public sealed class HydraulicSolverReferenceCaseTests
{
    [Fact]
    public void Solve_ReferenceCaseSingleBranchKnownPressureDifference_MatchesExpectedResiduals()
    {
        AssertReferenceCase(HydraulicSolverReferenceCases.SingleBranchKnownPressureDifference());
    }

    [Fact]
    public void Solve_ReferenceCaseTwoParallelBranches_MatchesExpectedFlowSplitAndResiduals()
    {
        AssertReferenceCase(HydraulicSolverReferenceCases.TwoParallelBranches());
    }

    [Fact]
    public void Solve_ReferenceCaseFixedPumpPressureIncrease_MatchesExpectedPressureResidual()
    {
        AssertReferenceCase(HydraulicSolverReferenceCases.FixedPumpPressureIncrease());
    }

    [Fact]
    public void Solve_ReferenceCaseMaxIterationsReached_ReturnsStatusWithOpenResiduals()
    {
        AssertReferenceCase(HydraulicSolverReferenceCases.MaxIterationsReached());
    }

    [Fact]
    public void Solve_ReferenceCaseInvalidInput_ReturnsStatusWithoutIterationHistory()
    {
        AssertReferenceCase(HydraulicSolverReferenceCases.InvalidInput());
    }

    [Fact]
    public void Solve_ReferenceCaseZeroFlowBoundaryCase_ConvergesWithoutDivisionByZero()
    {
        AssertReferenceCase(HydraulicSolverReferenceCases.ZeroFlowBoundaryCase());
    }

    private static void AssertReferenceCase(HydraulicSolverReferenceCase referenceCase)
    {
        var result = new SmallHydraulicNetworkSolver().Solve(referenceCase.Input);

        Assert.Equal(referenceCase.ExpectedStatus, result.Status);
        if (referenceCase.ExpectedIterations.HasValue)
        {
            Assert.Equal(referenceCase.ExpectedIterations.Value, result.Iterations);
        }

        if (referenceCase.ExpectIterationHistory)
        {
            Assert.NotEmpty(result.IterationHistory);
        }
        else
        {
            Assert.Empty(result.IterationHistory);
        }

        AssertBranchFlows(referenceCase, result.SolvedVolumetricFlowRatesCubicMetersPerSecond);
        AssertNodeResiduals(referenceCase, result.NodeBalances);
        AssertPressureResiduals(referenceCase, result.PressureResiduals);
        AssertMaxResiduals(referenceCase, result);
    }

    private static void AssertBranchFlows(
        HydraulicSolverReferenceCase referenceCase,
        IReadOnlyDictionary<string, double> actualFlows)
    {
        foreach (var expected in referenceCase.ExpectedBranchFlowsCubicMetersPerSecond)
        {
            Assert.True(
                actualFlows.ContainsKey(expected.Key),
                $"Reference case '{referenceCase.Id}' is missing branch flow '{expected.Key}'.");
            Assert.Equal(expected.Value, actualFlows[expected.Key], precision: 12);
        }
    }

    private static void AssertNodeResiduals(
        HydraulicSolverReferenceCase referenceCase,
        IReadOnlyList<HydraulicNodeBalance> actualBalances)
    {
        var actualByNode = actualBalances.ToDictionary(balance => balance.NodeId, StringComparer.Ordinal);

        foreach (var expected in referenceCase.ExpectedNodeResidualsCubicMetersPerSecond)
        {
            Assert.True(
                actualByNode.ContainsKey(expected.Key),
                $"Reference case '{referenceCase.Id}' is missing node balance '{expected.Key}'.");
            Assert.Equal(
                expected.Value,
                actualByNode[expected.Key].ResidualFlowCubicMetersPerSecond,
                precision: 12);
        }
    }

    private static void AssertPressureResiduals(
        HydraulicSolverReferenceCase referenceCase,
        IReadOnlyList<HydraulicPressureResidual> actualResiduals)
    {
        if (referenceCase.ExpectedPressureResidualsPascals.Count == 0
            && referenceCase.MinimumMaxPressureResidualPascals.HasValue)
        {
            return;
        }

        var actualByElement = actualResiduals.ToDictionary(residual => residual.ElementId, StringComparer.Ordinal);

        Assert.Equal(referenceCase.ExpectedPressureResidualsPascals.Count, actualResiduals.Count);
        foreach (var expected in referenceCase.ExpectedPressureResidualsPascals)
        {
            Assert.True(
                actualByElement.ContainsKey(expected.Key),
                $"Reference case '{referenceCase.Id}' is missing pressure residual '{expected.Key}'.");

            var actual = actualByElement[expected.Key];
            Assert.Equal(
                expected.Value.AvailablePressureIncreasePascals,
                actual.AvailablePressureIncreasePascals,
                precision: 6);
            Assert.Equal(
                expected.Value.RequiredPressureIncreasePascals,
                actual.RequiredPressureIncreasePascals,
                precision: 6);
            Assert.Equal(
                expected.Value.ResidualPressurePascals,
                actual.ResidualPressurePascals,
                precision: 6);
        }
    }

    private static void AssertMaxResiduals(
        HydraulicSolverReferenceCase referenceCase,
        HydraulicSolverResult result)
    {
        if (referenceCase.ExpectedMaxNodeResidualCubicMetersPerSecond.HasValue)
        {
            Assert.Equal(
                referenceCase.ExpectedMaxNodeResidualCubicMetersPerSecond.Value,
                result.MaxNodeBalanceResidualCubicMetersPerSecond,
                precision: 12);
        }

        if (referenceCase.ExpectedMaxPressureResidualPascals.HasValue)
        {
            Assert.Equal(
                referenceCase.ExpectedMaxPressureResidualPascals.Value,
                result.MaxPressureResidualPascals,
                precision: 6);
        }

        if (referenceCase.MinimumMaxNodeResidualCubicMetersPerSecond.HasValue)
        {
            Assert.True(
                result.MaxNodeBalanceResidualCubicMetersPerSecond
                    > referenceCase.MinimumMaxNodeResidualCubicMetersPerSecond.Value,
                $"Reference case '{referenceCase.Id}' expected an open node-balance residual.");
        }

        if (referenceCase.MinimumMaxPressureResidualPascals.HasValue)
        {
            Assert.True(
                result.MaxPressureResidualPascals
                    > referenceCase.MinimumMaxPressureResidualPascals.Value,
                $"Reference case '{referenceCase.Id}' expected an open pressure residual.");
        }
    }
}
