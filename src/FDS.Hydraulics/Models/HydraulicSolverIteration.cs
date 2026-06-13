using System.Collections.ObjectModel;

using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

public sealed class HydraulicSolverIteration
{
    public HydraulicSolverIteration(
        int iterationNumber,
        IReadOnlyDictionary<string, double> volumetricFlowRatesCubicMetersPerSecond,
        IEnumerable<HydraulicNodeBalance> nodeBalances,
        IEnumerable<HydraulicPressureResidual>? pressureResiduals = null)
    {
        if (iterationNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(iterationNumber), iterationNumber, "Iteration number cannot be negative.");
        }

        IterationNumber = iterationNumber;
        VolumetricFlowRatesCubicMetersPerSecond = ToReadOnlyDictionary(volumetricFlowRatesCubicMetersPerSecond);
        NodeBalances = ToReadOnlyList(nodeBalances, nameof(nodeBalances));
        PressureResiduals = ToReadOnlyList(pressureResiduals, nameof(pressureResiduals));
    }

    public int IterationNumber { get; }

    public IReadOnlyDictionary<string, double> VolumetricFlowRatesCubicMetersPerSecond { get; }

    public IReadOnlyList<HydraulicNodeBalance> NodeBalances { get; }

    public IReadOnlyList<HydraulicPressureResidual> PressureResiduals { get; }

    public double MaxNodeBalanceResidualCubicMetersPerSecond =>
        NodeBalances.Count == 0
            ? 0
            : NodeBalances.Max(balance => Math.Abs(balance.ResidualFlowCubicMetersPerSecond));

    public double MaxPressureResidualPascals =>
        PressureResiduals.Count == 0
            ? 0
            : PressureResiduals.Max(residual => Math.Abs(residual.ResidualPressurePascals));

    private static IReadOnlyDictionary<string, double> ToReadOnlyDictionary(
        IReadOnlyDictionary<string, double> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var items = values.ToDictionary(
            pair => HydraulicValidation.RequiredId(pair.Key, nameof(values)),
            pair => HydraulicValidation.EnsureNonNegativeFinite(pair.Value, nameof(values)),
            StringComparer.Ordinal);

        return new ReadOnlyDictionary<string, double>(items);
    }

    private static IReadOnlyList<T> ToReadOnlyList<T>(IEnumerable<T>? values, string parameterName)
        where T : class
    {
        if (values is null)
        {
            return Array.Empty<T>();
        }

        var items = values
            .Select(value => value ?? throw new ArgumentException("Iteration collections cannot contain null.", parameterName))
            .ToList();

        return new ReadOnlyCollection<T>(items);
    }
}
