using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Boundary condition descriptor for future hydraulic solvers. It stores input
/// constraints only; it does not calculate a flow, pressure, or control action.
/// </summary>
public sealed class HydraulicBoundaryCondition
{
    private HydraulicBoundaryCondition(
        string id,
        HydraulicBoundaryConditionKind kind,
        string? nodeId = null,
        string? fromNodeId = null,
        string? toNodeId = null,
        double? volumetricFlowRateCubicMetersPerSecond = null,
        double? pressurePascals = null,
        double? pressureDifferencePascals = null,
        Pump? pump = null)
    {
        Id = HydraulicValidation.RequiredId(id, nameof(id));
        Kind = kind;
        NodeId = nodeId is null ? null : HydraulicValidation.RequiredId(nodeId, nameof(nodeId));
        FromNodeId = fromNodeId is null ? null : HydraulicValidation.RequiredId(fromNodeId, nameof(fromNodeId));
        ToNodeId = toNodeId is null ? null : HydraulicValidation.RequiredId(toNodeId, nameof(toNodeId));
        VolumetricFlowRateCubicMetersPerSecond = volumetricFlowRateCubicMetersPerSecond;
        PressurePascals = pressurePascals;
        PressureDifferencePascals = pressureDifferencePascals;
        Pump = pump;
    }

    public string Id { get; }

    public HydraulicBoundaryConditionKind Kind { get; }

    public string? NodeId { get; }

    public string? FromNodeId { get; }

    public string? ToNodeId { get; }

    public double? VolumetricFlowRateCubicMetersPerSecond { get; }

    public double? PressurePascals { get; }

    public double? PressureDifferencePascals { get; }

    public Pump? Pump { get; }

    public static HydraulicBoundaryCondition SourceFlow(
        string id,
        string nodeId,
        double volumetricFlowRateCubicMetersPerSecond)
    {
        return new HydraulicBoundaryCondition(
            id,
            HydraulicBoundaryConditionKind.SourceFlow,
            nodeId: nodeId,
            volumetricFlowRateCubicMetersPerSecond: HydraulicValidation.EnsureNonNegativeFinite(
                volumetricFlowRateCubicMetersPerSecond,
                nameof(volumetricFlowRateCubicMetersPerSecond)));
    }

    public static HydraulicBoundaryCondition SinkFlow(
        string id,
        string nodeId,
        double volumetricFlowRateCubicMetersPerSecond)
    {
        return new HydraulicBoundaryCondition(
            id,
            HydraulicBoundaryConditionKind.SinkFlow,
            nodeId: nodeId,
            volumetricFlowRateCubicMetersPerSecond: HydraulicValidation.EnsureNonNegativeFinite(
                volumetricFlowRateCubicMetersPerSecond,
                nameof(volumetricFlowRateCubicMetersPerSecond)));
    }

    public static HydraulicBoundaryCondition KnownPressure(string id, string nodeId, double pressurePascals)
    {
        return new HydraulicBoundaryCondition(
            id,
            HydraulicBoundaryConditionKind.KnownPressure,
            nodeId: nodeId,
            pressurePascals: HydraulicValidation.EnsureFinite(pressurePascals, nameof(pressurePascals)));
    }

    public static HydraulicBoundaryCondition KnownPressureDifference(
        string id,
        string fromNodeId,
        string toNodeId,
        double pressureDifferencePascals)
    {
        return new HydraulicBoundaryCondition(
            id,
            HydraulicBoundaryConditionKind.KnownPressureDifference,
            fromNodeId: fromNodeId,
            toNodeId: toNodeId,
            pressureDifferencePascals: HydraulicValidation.EnsureFinite(
                pressureDifferencePascals,
                nameof(pressureDifferencePascals)));
    }

    public static HydraulicBoundaryCondition PumpCurve(string id, string fromNodeId, string toNodeId, Pump pump)
    {
        ArgumentNullException.ThrowIfNull(pump);

        return new HydraulicBoundaryCondition(
            id,
            HydraulicBoundaryConditionKind.PumpCurve,
            fromNodeId: fromNodeId,
            toNodeId: toNodeId,
            pump: pump);
    }
}
