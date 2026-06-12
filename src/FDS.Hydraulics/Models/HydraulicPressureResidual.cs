using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Pressure residual for a branch or edge. Residual convention is available
/// pressure increase minus required pressure increase.
/// </summary>
public sealed class HydraulicPressureResidual
{
    public HydraulicPressureResidual(
        string elementId,
        string elementKind,
        double availablePressureIncreasePascals,
        double requiredPressureIncreasePascals)
    {
        ElementId = HydraulicValidation.RequiredId(elementId, nameof(elementId));
        ElementKind = HydraulicValidation.RequiredName(elementKind, nameof(elementKind));
        AvailablePressureIncreasePascals = HydraulicValidation.EnsureNonNegativeFinite(
            availablePressureIncreasePascals,
            nameof(availablePressureIncreasePascals));
        RequiredPressureIncreasePascals = HydraulicValidation.EnsureNonNegativeFinite(
            requiredPressureIncreasePascals,
            nameof(requiredPressureIncreasePascals));
    }

    public string ElementId { get; }

    public string ElementKind { get; }

    public double AvailablePressureIncreasePascals { get; }

    public double RequiredPressureIncreasePascals { get; }

    public double ResidualPressurePascals =>
        AvailablePressureIncreasePascals - RequiredPressureIncreasePascals;
}
