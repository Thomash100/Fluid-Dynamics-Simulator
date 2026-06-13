using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Flow balance at one node. Residual convention is inflow plus source flow
/// minus outflow minus sink flow.
/// </summary>
public sealed class HydraulicNodeBalance
{
    public HydraulicNodeBalance(
        string nodeId,
        double incomingFlowCubicMetersPerSecond,
        double outgoingFlowCubicMetersPerSecond,
        double sourceFlowCubicMetersPerSecond = 0,
        double sinkFlowCubicMetersPerSecond = 0)
    {
        NodeId = HydraulicValidation.RequiredId(nodeId, nameof(nodeId));
        IncomingFlowCubicMetersPerSecond = HydraulicValidation.EnsureNonNegativeFinite(
            incomingFlowCubicMetersPerSecond,
            nameof(incomingFlowCubicMetersPerSecond));
        OutgoingFlowCubicMetersPerSecond = HydraulicValidation.EnsureNonNegativeFinite(
            outgoingFlowCubicMetersPerSecond,
            nameof(outgoingFlowCubicMetersPerSecond));
        SourceFlowCubicMetersPerSecond = HydraulicValidation.EnsureNonNegativeFinite(
            sourceFlowCubicMetersPerSecond,
            nameof(sourceFlowCubicMetersPerSecond));
        SinkFlowCubicMetersPerSecond = HydraulicValidation.EnsureNonNegativeFinite(
            sinkFlowCubicMetersPerSecond,
            nameof(sinkFlowCubicMetersPerSecond));
    }

    public string NodeId { get; }

    public double IncomingFlowCubicMetersPerSecond { get; }

    public double OutgoingFlowCubicMetersPerSecond { get; }

    public double SourceFlowCubicMetersPerSecond { get; }

    public double SinkFlowCubicMetersPerSecond { get; }

    public double ResidualFlowCubicMetersPerSecond =>
        IncomingFlowCubicMetersPerSecond
        + SourceFlowCubicMetersPerSecond
        - OutgoingFlowCubicMetersPerSecond
        - SinkFlowCubicMetersPerSecond;
}
