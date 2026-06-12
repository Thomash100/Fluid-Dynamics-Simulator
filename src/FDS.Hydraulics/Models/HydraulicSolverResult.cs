using System.Collections.ObjectModel;

using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Solver result containing final residuals and optional iteration history.
/// </summary>
public sealed class HydraulicSolverResult
{
    public HydraulicSolverResult(
        HydraulicSolverStatus status,
        int iterations,
        IEnumerable<HydraulicNodeBalance> nodeBalances,
        IEnumerable<HydraulicPressureResidual>? pressureResiduals = null,
        IEnumerable<HydraulicBoundaryCondition>? boundaryConditions = null,
        IEnumerable<HydraulicSolverIteration>? iterationHistory = null,
        IReadOnlyDictionary<string, double>? solvedVolumetricFlowRatesCubicMetersPerSecond = null)
    {
        if (iterations < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations), iterations, "Iterations cannot be negative.");
        }

        Status = status;
        Iterations = iterations;
        NodeBalances = ToReadOnlyList(nodeBalances, nameof(nodeBalances));
        PressureResiduals = ToReadOnlyList(pressureResiduals, nameof(pressureResiduals));
        BoundaryConditions = ToReadOnlyList(boundaryConditions, nameof(boundaryConditions));
        IterationHistory = ToReadOnlyList(iterationHistory, nameof(iterationHistory));
        SolvedVolumetricFlowRatesCubicMetersPerSecond = ToReadOnlyDictionary(
            solvedVolumetricFlowRatesCubicMetersPerSecond);
    }

    public HydraulicSolverStatus Status { get; }

    public int Iterations { get; }

    public IReadOnlyList<HydraulicNodeBalance> NodeBalances { get; }

    public IReadOnlyList<HydraulicPressureResidual> PressureResiduals { get; }

    public IReadOnlyList<HydraulicBoundaryCondition> BoundaryConditions { get; }

    public IReadOnlyList<HydraulicSolverIteration> IterationHistory { get; }

    public IReadOnlyDictionary<string, double> SolvedVolumetricFlowRatesCubicMetersPerSecond { get; }

    public double MaxNodeBalanceResidualCubicMetersPerSecond =>
        NodeBalances.Count == 0
            ? 0
            : NodeBalances.Max(balance => Math.Abs(balance.ResidualFlowCubicMetersPerSecond));

    public double MaxPressureResidualPascals =>
        PressureResiduals.Count == 0
            ? 0
            : PressureResiduals.Max(residual => Math.Abs(residual.ResidualPressurePascals));

    private static IReadOnlyList<T> ToReadOnlyList<T>(IEnumerable<T>? values, string parameterName)
        where T : class
    {
        if (values is null)
        {
            return Array.Empty<T>();
        }

        var items = values
            .Select(value => value ?? throw new ArgumentException("Result collections cannot contain null.", parameterName))
            .ToList();

        return new ReadOnlyCollection<T>(items);
    }

    private static IReadOnlyDictionary<string, double> ToReadOnlyDictionary(
        IReadOnlyDictionary<string, double>? values)
    {
        if (values is null)
        {
            return new ReadOnlyDictionary<string, double>(
                new Dictionary<string, double>(StringComparer.Ordinal));
        }

        var items = values.ToDictionary(
            pair => HydraulicValidation.RequiredId(pair.Key, nameof(values)),
            pair => HydraulicValidation.EnsureNonNegativeFinite(pair.Value, nameof(values)),
            StringComparer.Ordinal);

        return new ReadOnlyDictionary<string, double>(items);
    }
}
