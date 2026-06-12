using System.Collections.ObjectModel;

using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Solver preparation result containing residuals. It is not the output of a
/// completed iterative solver.
/// </summary>
public sealed class HydraulicSolverResult
{
    public HydraulicSolverResult(
        HydraulicSolverStatus status,
        int iterations,
        IEnumerable<HydraulicNodeBalance> nodeBalances,
        IEnumerable<HydraulicPressureResidual>? pressureResiduals = null,
        IEnumerable<HydraulicBoundaryCondition>? boundaryConditions = null)
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
    }

    public HydraulicSolverStatus Status { get; }

    public int Iterations { get; }

    public IReadOnlyList<HydraulicNodeBalance> NodeBalances { get; }

    public IReadOnlyList<HydraulicPressureResidual> PressureResiduals { get; }

    public IReadOnlyList<HydraulicBoundaryCondition> BoundaryConditions { get; }

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
}
