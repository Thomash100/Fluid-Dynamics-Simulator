using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Generic local resistance described by a dimensionless zeta value.
/// </summary>
public sealed class LocalResistance
{
    public LocalResistance(string id, string name, double zeta)
    {
        Id = HydraulicValidation.RequiredId(id, nameof(id));
        Name = HydraulicValidation.RequiredName(name, nameof(name));
        Zeta = HydraulicValidation.EnsureNonNegativeFinite(zeta, nameof(zeta));
    }

    public string Id { get; }

    public string Name { get; }

    /// <summary>
    /// Dimensionless local loss coefficient.
    /// </summary>
    public double Zeta { get; }
}
