using System.Collections.ObjectModel;

using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Input for the small reference solver. Branch flow estimates are initial
/// values; the solver may update them during iteration.
/// </summary>
public sealed class HydraulicSolverInput
{
    public HydraulicSolverInput(
        Network topology,
        IEnumerable<HydraulicBranch> branches,
        Fluid fluid,
        double dynamicViscosityPascalSeconds,
        IEnumerable<HydraulicBoundaryCondition>? boundaryConditions = null,
        HydraulicSolverOptions? options = null,
        IReadOnlyDictionary<string, double>? initialVolumetricFlowRatesCubicMetersPerSecond = null,
        double gravitationalAccelerationMetersPerSecondSquared = PumpCalculator.StandardGravityMetersPerSecondSquared)
    {
        Topology = topology ?? throw new ArgumentNullException(nameof(topology));
        Fluid = fluid ?? throw new ArgumentNullException(nameof(fluid));
        DynamicViscosityPascalSeconds = HydraulicValidation.EnsurePositiveFinite(
            dynamicViscosityPascalSeconds,
            nameof(dynamicViscosityPascalSeconds));
        GravitationalAccelerationMetersPerSecondSquared = HydraulicValidation.EnsurePositiveFinite(
            gravitationalAccelerationMetersPerSecondSquared,
            nameof(gravitationalAccelerationMetersPerSecondSquared));
        Branches = ToReadOnlyList(branches, nameof(branches));

        if (Branches.Count == 0)
        {
            throw new ArgumentException("Solver input requires at least one branch.", nameof(branches));
        }

        ValidateUniqueBranchIds(Branches);

        BoundaryConditions = ToReadOnlyList(boundaryConditions, nameof(boundaryConditions));
        Options = options ?? HydraulicSolverOptions.Default;
        InitialVolumetricFlowRatesCubicMetersPerSecond = ToReadOnlyDictionary(
            initialVolumetricFlowRatesCubicMetersPerSecond);
    }

    public Network Topology { get; }

    public IReadOnlyList<HydraulicBranch> Branches { get; }

    public Fluid Fluid { get; }

    public double DynamicViscosityPascalSeconds { get; }

    public IReadOnlyList<HydraulicBoundaryCondition> BoundaryConditions { get; }

    public HydraulicSolverOptions Options { get; }

    public IReadOnlyDictionary<string, double> InitialVolumetricFlowRatesCubicMetersPerSecond { get; }

    public double GravitationalAccelerationMetersPerSecondSquared { get; }

    private static IReadOnlyList<T> ToReadOnlyList<T>(IEnumerable<T>? values, string parameterName)
        where T : class
    {
        if (values is null)
        {
            return Array.Empty<T>();
        }

        var items = values
            .Select(value => value ?? throw new ArgumentException("Solver input collections cannot contain null.", parameterName))
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

    private static void ValidateUniqueBranchIds(IReadOnlyList<HydraulicBranch> branches)
    {
        var duplicateBranchId = branches
            .GroupBy(branch => branch.Id, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1)
            ?.Key;

        if (duplicateBranchId is not null)
        {
            throw new ArgumentException($"Branch IDs must be unique. Duplicate ID: {duplicateBranchId}", nameof(branches));
        }
    }
}
