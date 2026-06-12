using FDS.Core.Models;
using FDS.Hydraulics.Internal;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Calculations;

/// <summary>
/// Prepares residual data for a future iterative hydraulic solver. It does not
/// iterate, change flow estimates, or choose pumps.
/// </summary>
public static class HydraulicSolverPreparationCalculator
{
    public static HydraulicSolverResult Prepare(
        Network topology,
        IEnumerable<HydraulicBoundaryCondition>? boundaryConditions = null,
        HydraulicSolverOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(topology);

        _ = options ?? HydraulicSolverOptions.Default;
        var boundaries = ToBoundaryList(boundaryConditions);
        ValidateBoundaryReferences(topology, boundaries);

        return new HydraulicSolverResult(
            HydraulicSolverStatus.Prepared,
            iterations: 0,
            CalculateNodeBalances(topology, boundaries),
            boundaryConditions: boundaries);
    }

    public static HydraulicSolverResult Prepare(
        Network topology,
        HydraulicNetwork hydraulicNetwork,
        Fluid fluid,
        double dynamicViscosityPascalSeconds,
        IEnumerable<HydraulicBoundaryCondition>? boundaryConditions = null,
        HydraulicSolverOptions? options = null,
        double gravitationalAccelerationMetersPerSecondSquared = PumpCalculator.StandardGravityMetersPerSecondSquared)
    {
        ArgumentNullException.ThrowIfNull(topology);
        ArgumentNullException.ThrowIfNull(hydraulicNetwork);
        ArgumentNullException.ThrowIfNull(fluid);
        HydraulicValidation.EnsurePositiveFinite(
            dynamicViscosityPascalSeconds,
            nameof(dynamicViscosityPascalSeconds));
        HydraulicValidation.EnsurePositiveFinite(
            gravitationalAccelerationMetersPerSecondSquared,
            nameof(gravitationalAccelerationMetersPerSecondSquared));

        _ = options ?? HydraulicSolverOptions.Default;
        var boundaries = ToBoundaryList(boundaryConditions);
        ValidateBoundaryReferences(topology, boundaries);

        var networkResult = HydraulicNetworkCalculator.Calculate(
            hydraulicNetwork,
            fluid,
            dynamicViscosityPascalSeconds,
            gravitationalAccelerationMetersPerSecondSquared);

        return new HydraulicSolverResult(
            HydraulicSolverStatus.Prepared,
            iterations: 0,
            CalculateNodeBalances(topology, boundaries),
            CalculatePressureResiduals(networkResult),
            boundaries);
    }

    private static IReadOnlyList<HydraulicBoundaryCondition> ToBoundaryList(
        IEnumerable<HydraulicBoundaryCondition>? boundaryConditions)
    {
        if (boundaryConditions is null)
        {
            return Array.Empty<HydraulicBoundaryCondition>();
        }

        return boundaryConditions
            .Select(boundary => boundary ?? throw new ArgumentException("Boundary conditions cannot contain null.", nameof(boundaryConditions)))
            .ToList();
    }

    private static IReadOnlyList<HydraulicNodeBalance> CalculateNodeBalances(
        Network topology,
        IReadOnlyList<HydraulicBoundaryCondition> boundaryConditions)
    {
        var entries = topology.Nodes.Keys.ToDictionary(
            nodeId => nodeId,
            _ => new MutableNodeBalance(),
            StringComparer.Ordinal);

        foreach (var edge in topology.Edges.Values)
        {
            AddEdgeFlow(entries, edge);
        }

        foreach (var boundary in boundaryConditions)
        {
            AddBoundaryFlow(entries, boundary);
        }

        return entries
            .OrderBy(entry => entry.Key, StringComparer.Ordinal)
            .Select(entry => entry.Value.ToBalance(entry.Key))
            .ToList();
    }

    private static IReadOnlyList<HydraulicPressureResidual> CalculatePressureResiduals(
        HydraulicNetworkResult networkResult)
    {
        return networkResult.BranchResults
            .Select(result => new HydraulicPressureResidual(
                result.BranchId,
                "Branch",
                result.BranchResult.PumpPressureIncreasePascals,
                result.TotalPressureLossPascals))
            .ToList();
    }

    private static void AddEdgeFlow(IDictionary<string, MutableNodeBalance> entries, Edge edge)
    {
        var flow = edge.VolumetricFlowRateCubicMetersPerSecond ?? 0;
        HydraulicValidation.EnsureFinite(flow, nameof(edge.VolumetricFlowRateCubicMetersPerSecond));

        if (flow >= 0)
        {
            entries[edge.FromNodeId].Outgoing += flow;
            entries[edge.ToNodeId].Incoming += flow;
            return;
        }

        var reverseFlow = Math.Abs(flow);
        entries[edge.FromNodeId].Incoming += reverseFlow;
        entries[edge.ToNodeId].Outgoing += reverseFlow;
    }

    private static void AddBoundaryFlow(
        IDictionary<string, MutableNodeBalance> entries,
        HydraulicBoundaryCondition boundary)
    {
        if (boundary.Kind == HydraulicBoundaryConditionKind.SourceFlow)
        {
            entries[boundary.NodeId!].Source += boundary.VolumetricFlowRateCubicMetersPerSecond!.Value;
        }
        else if (boundary.Kind == HydraulicBoundaryConditionKind.SinkFlow)
        {
            entries[boundary.NodeId!].Sink += boundary.VolumetricFlowRateCubicMetersPerSecond!.Value;
        }
    }

    private static void ValidateBoundaryReferences(
        Network topology,
        IReadOnlyList<HydraulicBoundaryCondition> boundaryConditions)
    {
        foreach (var boundary in boundaryConditions)
        {
            if (boundary.NodeId is not null && !topology.Nodes.ContainsKey(boundary.NodeId))
            {
                throw new ArgumentException($"Boundary condition '{boundary.Id}' references unknown node '{boundary.NodeId}'.", nameof(boundaryConditions));
            }

            if (boundary.FromNodeId is not null && !topology.Nodes.ContainsKey(boundary.FromNodeId))
            {
                throw new ArgumentException($"Boundary condition '{boundary.Id}' references unknown from-node '{boundary.FromNodeId}'.", nameof(boundaryConditions));
            }

            if (boundary.ToNodeId is not null && !topology.Nodes.ContainsKey(boundary.ToNodeId))
            {
                throw new ArgumentException($"Boundary condition '{boundary.Id}' references unknown to-node '{boundary.ToNodeId}'.", nameof(boundaryConditions));
            }
        }
    }

    private sealed class MutableNodeBalance
    {
        public double Incoming { get; set; }

        public double Outgoing { get; set; }

        public double Source { get; set; }

        public double Sink { get; set; }

        public HydraulicNodeBalance ToBalance(string nodeId)
        {
            return new HydraulicNodeBalance(nodeId, Incoming, Outgoing, Source, Sink);
        }
    }
}
