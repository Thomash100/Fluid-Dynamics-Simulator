namespace FDS.Hydraulics.Models;

public enum HydraulicBoundaryConditionKind
{
    SourceFlow,
    SinkFlow,
    KnownPressure,
    KnownPressureDifference,
    PumpCurve
}
