using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Minimal valve model. It may carry either a zeta resistance, Kv/Kvs data, or
/// both. It does not perform control-valve sizing.
/// </summary>
public sealed class Valve
{
    public Valve(
        string id,
        string name,
        LocalResistance? resistance = null,
        ValveFlowCoefficient? flowCoefficient = null)
    {
        Id = HydraulicValidation.RequiredId(id, nameof(id));
        Name = HydraulicValidation.RequiredName(name, nameof(name));
        Resistance = resistance;
        FlowCoefficient = flowCoefficient;
    }

    public string Id { get; }

    public string Name { get; }

    public LocalResistance? Resistance { get; }

    public ValveFlowCoefficient? FlowCoefficient { get; }
}
