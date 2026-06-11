using System.Collections.ObjectModel;

using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Simple hydraulic branch for evaluating known components at a supplied flow
/// rate. Local losses use one reference pipe cross-section; this is not a
/// segment-level or network model.
/// </summary>
public sealed class HydraulicBranch
{
    public HydraulicBranch(
        string id,
        string name,
        IEnumerable<Pipe> pipes,
        IEnumerable<LocalResistance>? localResistances = null,
        IEnumerable<Fitting>? fittings = null,
        IEnumerable<Valve>? valves = null,
        Pump? pump = null,
        Pipe? localResistanceReferencePipe = null)
    {
        Id = HydraulicValidation.RequiredId(id, nameof(id));
        Name = HydraulicValidation.RequiredName(name, nameof(name));
        Pipes = ToReadOnlyList(pipes, nameof(pipes));

        if (Pipes.Count == 0)
        {
            throw new ArgumentException("A hydraulic branch requires at least one pipe.", nameof(pipes));
        }

        LocalResistances = ToReadOnlyList(localResistances, nameof(localResistances));
        Fittings = ToReadOnlyList(fittings, nameof(fittings));
        Valves = ToReadOnlyList(valves, nameof(valves));
        Pump = pump;
        LocalResistanceReferencePipe = localResistanceReferencePipe ?? Pipes[0];

        if (!Pipes.Any(pipe => pipe.Id == LocalResistanceReferencePipe.Id))
        {
            throw new ArgumentException("The local resistance reference pipe must belong to the branch.", nameof(localResistanceReferencePipe));
        }
    }

    public string Id { get; }

    public string Name { get; }

    public IReadOnlyList<Pipe> Pipes { get; }

    public IReadOnlyList<LocalResistance> LocalResistances { get; }

    public IReadOnlyList<Fitting> Fittings { get; }

    public IReadOnlyList<Valve> Valves { get; }

    public Pump? Pump { get; }

    public Pipe LocalResistanceReferencePipe { get; }

    private static IReadOnlyList<T> ToReadOnlyList<T>(IEnumerable<T>? values, string parameterName)
        where T : class
    {
        if (values is null)
        {
            return Array.Empty<T>();
        }

        var items = values
            .Select(value => value ?? throw new ArgumentException("Component collections cannot contain null.", parameterName))
            .ToList();

        return new ReadOnlyCollection<T>(items);
    }
}
