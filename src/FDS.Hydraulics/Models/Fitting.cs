using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

public enum FittingKind
{
    Generic,
    Bend,
    Tee,
    Reducer,
    Expansion,
    Contraction
}

/// <summary>
/// Basic fitting model backed by a local resistance coefficient.
/// </summary>
public sealed class Fitting
{
    public Fitting(string id, string name, double zeta, FittingKind kind = FittingKind.Generic)
    {
        Id = HydraulicValidation.RequiredId(id, nameof(id));
        Name = HydraulicValidation.RequiredName(name, nameof(name));
        Kind = kind;
        Resistance = new LocalResistance(Id, Name, zeta);
    }

    public string Id { get; }

    public string Name { get; }

    public FittingKind Kind { get; }

    public LocalResistance Resistance { get; }
}
