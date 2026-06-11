using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Basic pump model with a head curve and optional efficiency curve. It does
/// not calculate an automatic operating point.
/// </summary>
public sealed class Pump
{
    public Pump(string id, string name, PumpCurve curve, PumpEfficiencyCurve? efficiencyCurve = null)
    {
        Id = HydraulicValidation.RequiredId(id, nameof(id));
        Name = HydraulicValidation.RequiredName(name, nameof(name));
        Curve = curve ?? throw new ArgumentNullException(nameof(curve));
        EfficiencyCurve = efficiencyCurve;
    }

    public string Id { get; }

    public string Name { get; }

    public PumpCurve Curve { get; }

    public PumpEfficiencyCurve? EfficiencyCurve { get; }
}
