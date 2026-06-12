using FDS.Core.Models;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Calculations;

/// <summary>
/// Small reference solver for prepared hydraulic networks. It uses simple
/// relaxation of branch flow estimates against node-balance and pressure
/// residuals. It is intentionally not a general Newton, Hardy-Cross, or
/// gradient solver.
/// </summary>
public sealed class SmallHydraulicNetworkSolver : IHydraulicNetworkSolver
{
    private const double MinimumSeedFlowCubicMetersPerSecond = 1e-6;

    public HydraulicSolverResult Solve(HydraulicSolverInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (!TryCreateBranchStates(input, out var branchStates, out var invalidResult))
        {
            return invalidResult!;
        }

        var flowRates = CreateInitialFlowRates(input);
        if (flowRates.Keys.Any(branchId => !branchStates.ContainsKey(branchId)))
        {
            return CreateInvalidInputResult(input);
        }

        var iterationHistory = new List<HydraulicSolverIteration>();

        for (var iterationNumber = 0; iterationNumber <= input.Options.MaxIterations; iterationNumber++)
        {
            var nodeBalances = CalculateNodeBalances(input, branchStates.Values, flowRates);
            var pressureResiduals = CalculatePressureResiduals(input, branchStates.Values, flowRates);
            var iteration = new HydraulicSolverIteration(
                iterationNumber,
                flowRates,
                nodeBalances,
                pressureResiduals);
            iterationHistory.Add(iteration);

            if (IsConverged(iteration, input.Options))
            {
                return CreateResult(
                    HydraulicSolverStatus.Converged,
                    iterationNumber,
                    iteration,
                    input.BoundaryConditions,
                    iterationHistory,
                    flowRates);
            }

            if (iterationNumber == input.Options.MaxIterations)
            {
                return CreateResult(
                    HydraulicSolverStatus.MaxIterationsReached,
                    iterationNumber,
                    iteration,
                    input.BoundaryConditions,
                    iterationHistory,
                    flowRates);
            }

            flowRates = CalculateNextFlowRates(input, branchStates.Values, flowRates, nodeBalances);
        }

        var lastIteration = iterationHistory[^1];
        return CreateResult(
            HydraulicSolverStatus.MaxIterationsReached,
            input.Options.MaxIterations,
            lastIteration,
            input.BoundaryConditions,
            iterationHistory,
            flowRates);
    }

    private static Dictionary<string, double> CreateInitialFlowRates(HydraulicSolverInput input)
    {
        return input.Branches.ToDictionary(
            branch => branch.Id,
            branch => input.InitialVolumetricFlowRatesCubicMetersPerSecond.TryGetValue(branch.Id, out var flowRate)
                ? flowRate
                : 0,
            StringComparer.Ordinal);
    }

    private static bool TryCreateBranchStates(
        HydraulicSolverInput input,
        out IReadOnlyDictionary<string, BranchState> branchStates,
        out HydraulicSolverResult? invalidResult)
    {
        invalidResult = null;
        var pressureByNode = new Dictionary<string, double>(StringComparer.Ordinal);
        var knownPressureDifferences = new List<HydraulicBoundaryCondition>();
        var pumpBoundaries = new List<HydraulicBoundaryCondition>();

        foreach (var boundary in input.BoundaryConditions)
        {
            if (!BoundaryReferencesAreValid(input.Topology, boundary))
            {
                branchStates = new Dictionary<string, BranchState>(StringComparer.Ordinal);
                invalidResult = CreateInvalidInputResult(input);
                return false;
            }

            if (boundary.Kind == HydraulicBoundaryConditionKind.KnownPressure)
            {
                pressureByNode[boundary.NodeId!] = boundary.PressurePascals!.Value;
            }
            else if (boundary.Kind == HydraulicBoundaryConditionKind.KnownPressureDifference)
            {
                knownPressureDifferences.Add(boundary);
            }
            else if (boundary.Kind == HydraulicBoundaryConditionKind.PumpCurve)
            {
                pumpBoundaries.Add(boundary);
            }
        }

        var states = new List<BranchState>();
        foreach (var branch in input.Branches)
        {
            if (!TryGetBranchEndpoints(branch, out var fromNodeId, out var toNodeId)
                || !input.Topology.Nodes.ContainsKey(fromNodeId)
                || !input.Topology.Nodes.ContainsKey(toNodeId))
            {
                branchStates = new Dictionary<string, BranchState>(StringComparer.Ordinal);
                invalidResult = CreateInvalidInputResult(input);
                return false;
            }

            if (!TryCreatePressureProvider(
                fromNodeId,
                toNodeId,
                pressureByNode,
                knownPressureDifferences,
                pumpBoundaries,
                input,
                out var pressureProvider))
            {
                branchStates = new Dictionary<string, BranchState>(StringComparer.Ordinal);
                invalidResult = CreateInvalidInputResult(input);
                return false;
            }

            states.Add(new BranchState(branch, fromNodeId, toNodeId, pressureProvider));
        }

        var parallelCounts = states
            .GroupBy(state => state.DirectedNodePairKey, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);

        foreach (var state in states)
        {
            state.ParallelBranchCount = parallelCounts[state.DirectedNodePairKey];
        }

        branchStates = states.ToDictionary(state => state.Branch.Id, StringComparer.Ordinal);
        return true;
    }

    private static bool TryGetBranchEndpoints(HydraulicBranch branch, out string fromNodeId, out string toNodeId)
    {
        fromNodeId = branch.Pipes.FirstOrDefault()?.FromNodeId ?? string.Empty;
        toNodeId = branch.Pipes.LastOrDefault()?.ToNodeId ?? string.Empty;

        return !string.IsNullOrWhiteSpace(fromNodeId)
            && !string.IsNullOrWhiteSpace(toNodeId);
    }

    private static bool TryCreatePressureProvider(
        string fromNodeId,
        string toNodeId,
        IReadOnlyDictionary<string, double> pressureByNode,
        IReadOnlyList<HydraulicBoundaryCondition> knownPressureDifferences,
        IReadOnlyList<HydraulicBoundaryCondition> pumpBoundaries,
        HydraulicSolverInput input,
        out PressureProvider pressureProvider)
    {
        var constantPressureIncreasePascals = 0d;
        if (pressureByNode.TryGetValue(fromNodeId, out var fromPressure)
            && pressureByNode.TryGetValue(toNodeId, out var toPressure))
        {
            constantPressureIncreasePascals += fromPressure - toPressure;
        }

        foreach (var boundary in knownPressureDifferences)
        {
            if (boundary.FromNodeId == fromNodeId && boundary.ToNodeId == toNodeId)
            {
                constantPressureIncreasePascals += boundary.PressureDifferencePascals!.Value;
            }
            else if (boundary.FromNodeId == toNodeId && boundary.ToNodeId == fromNodeId)
            {
                constantPressureIncreasePascals -= boundary.PressureDifferencePascals!.Value;
            }
        }

        var pumps = pumpBoundaries
            .Where(boundary => boundary.FromNodeId == fromNodeId && boundary.ToNodeId == toNodeId)
            .Select(boundary => boundary.Pump!)
            .ToList();

        if (pumpBoundaries.Any(boundary => boundary.FromNodeId == toNodeId && boundary.ToNodeId == fromNodeId))
        {
            pressureProvider = PressureProvider.None;
            return false;
        }

        if (constantPressureIncreasePascals < 0)
        {
            pressureProvider = PressureProvider.None;
            return false;
        }

        pressureProvider = new PressureProvider(
            constantPressureIncreasePascals,
            pumps,
            input.Fluid,
            input.GravitationalAccelerationMetersPerSecondSquared);
        return true;
    }

    private static bool BoundaryReferencesAreValid(Network topology, HydraulicBoundaryCondition boundary)
    {
        return (boundary.NodeId is null || topology.Nodes.ContainsKey(boundary.NodeId))
            && (boundary.FromNodeId is null || topology.Nodes.ContainsKey(boundary.FromNodeId))
            && (boundary.ToNodeId is null || topology.Nodes.ContainsKey(boundary.ToNodeId));
    }

    private static IReadOnlyList<HydraulicNodeBalance> CalculateNodeBalances(
        HydraulicSolverInput input,
        IEnumerable<BranchState> branchStates,
        IReadOnlyDictionary<string, double> flowRates)
    {
        var entries = input.Topology.Nodes.Keys.ToDictionary(
            nodeId => nodeId,
            _ => new MutableNodeBalance(),
            StringComparer.Ordinal);

        foreach (var state in branchStates)
        {
            var flowRate = flowRates[state.Branch.Id];
            entries[state.FromNodeId].Outgoing += flowRate;
            entries[state.ToNodeId].Incoming += flowRate;
        }

        foreach (var boundary in input.BoundaryConditions)
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

        return entries
            .OrderBy(entry => entry.Key, StringComparer.Ordinal)
            .Select(entry => entry.Value.ToBalance(entry.Key))
            .ToList();
    }

    private static IReadOnlyList<HydraulicPressureResidual> CalculatePressureResiduals(
        HydraulicSolverInput input,
        IEnumerable<BranchState> branchStates,
        IReadOnlyDictionary<string, double> flowRates)
    {
        var residuals = new List<HydraulicPressureResidual>();

        foreach (var state in branchStates)
        {
            var flowRate = flowRates[state.Branch.Id];
            var branchResult = HydraulicBranchCalculator.Calculate(
                state.Branch,
                input.Fluid,
                flowRate,
                input.DynamicViscosityPascalSeconds,
                input.GravitationalAccelerationMetersPerSecondSquared);
            var externalPressureIncrease = state.PressureProvider.CalculatePressureIncreasePascals(flowRate);
            var availablePressureIncrease =
                externalPressureIncrease + branchResult.PumpPressureIncreasePascals;

            if (!state.PressureProvider.HasExternalPressure && branchResult.PumpPressureIncreasePascals == 0)
            {
                continue;
            }

            residuals.Add(new HydraulicPressureResidual(
                state.Branch.Id,
                "Branch",
                availablePressureIncrease,
                branchResult.PipePressureLossPascals + branchResult.LocalPressureLossPascals));
        }

        return residuals;
    }

    private static Dictionary<string, double> CalculateNextFlowRates(
        HydraulicSolverInput input,
        IEnumerable<BranchState> branchStates,
        IReadOnlyDictionary<string, double> currentFlowRates,
        IReadOnlyList<HydraulicNodeBalance> nodeBalances)
    {
        var residualsByNode = nodeBalances.ToDictionary(
            balance => balance.NodeId,
            balance => balance.ResidualFlowCubicMetersPerSecond,
            StringComparer.Ordinal);
        var nextFlowRates = new Dictionary<string, double>(currentFlowRates, StringComparer.Ordinal);

        foreach (var state in branchStates)
        {
            var currentFlow = currentFlowRates[state.Branch.Id];
            var balanceCorrection = (residualsByNode[state.FromNodeId] - residualsByNode[state.ToNodeId])
                / (2 * state.ParallelBranchCount);
            var correctedFlow = Math.Max(
                0,
                currentFlow + input.Options.RelaxationFactor * balanceCorrection);

            nextFlowRates[state.Branch.Id] = correctedFlow;
        }

        foreach (var state in branchStates)
        {
            var currentFlow = nextFlowRates[state.Branch.Id];
            var targetPressureIncrease = state.PressureProvider.CalculatePressureIncreasePascals(currentFlow);
            if (!state.PressureProvider.HasExternalPressure && state.Branch.Pump is null)
            {
                continue;
            }

            if (state.Branch.Pump is not null)
            {
                targetPressureIncrease += PumpCalculator.CalculatePressureIncreasePascals(
                    state.Branch.Pump,
                    input.Fluid,
                    currentFlow,
                    input.GravitationalAccelerationMetersPerSecondSquared);
            }

            var pressureMatchedFlow = EstimatePressureMatchedFlow(
                input,
                state.Branch,
                currentFlow,
                targetPressureIncrease);
            nextFlowRates[state.Branch.Id] = Math.Max(
                0,
                currentFlow + input.Options.RelaxationFactor * (pressureMatchedFlow - currentFlow));
        }

        return nextFlowRates;
    }

    private static double EstimatePressureMatchedFlow(
        HydraulicSolverInput input,
        HydraulicBranch branch,
        double currentFlow,
        double targetPressureIncreasePascals)
    {
        if (targetPressureIncreasePascals <= 0)
        {
            return 0;
        }

        var estimatedFlow = Math.Max(currentFlow, MinimumSeedFlowCubicMetersPerSecond);
        for (var index = 0; index < 4; index++)
        {
            var branchResult = HydraulicBranchCalculator.Calculate(
                branch,
                input.Fluid,
                estimatedFlow,
                input.DynamicViscosityPascalSeconds,
                input.GravitationalAccelerationMetersPerSecondSquared);
            var requiredPressureIncrease =
                branchResult.PipePressureLossPascals + branchResult.LocalPressureLossPascals;

            if (requiredPressureIncrease <= 0)
            {
                return estimatedFlow;
            }

            estimatedFlow *= Math.Sqrt(targetPressureIncreasePascals / requiredPressureIncrease);
        }

        return estimatedFlow;
    }

    private static bool IsConverged(HydraulicSolverIteration iteration, HydraulicSolverOptions options)
    {
        return iteration.MaxNodeBalanceResidualCubicMetersPerSecond
                <= options.FlowResidualToleranceCubicMetersPerSecond
            && iteration.MaxPressureResidualPascals
                <= options.PressureResidualTolerancePascals;
    }

    private static HydraulicSolverResult CreateResult(
        HydraulicSolverStatus status,
        int iterations,
        HydraulicSolverIteration finalIteration,
        IReadOnlyList<HydraulicBoundaryCondition> boundaryConditions,
        IReadOnlyList<HydraulicSolverIteration> iterationHistory,
        IReadOnlyDictionary<string, double> flowRates)
    {
        return new HydraulicSolverResult(
            status,
            iterations,
            finalIteration.NodeBalances,
            finalIteration.PressureResiduals,
            boundaryConditions,
            iterationHistory,
            flowRates);
    }

    private static HydraulicSolverResult CreateInvalidInputResult(HydraulicSolverInput input)
    {
        return new HydraulicSolverResult(
            HydraulicSolverStatus.InvalidInput,
            iterations: 0,
            Array.Empty<HydraulicNodeBalance>(),
            boundaryConditions: input.BoundaryConditions);
    }

    private sealed class BranchState
    {
        public BranchState(
            HydraulicBranch branch,
            string fromNodeId,
            string toNodeId,
            PressureProvider pressureProvider)
        {
            Branch = branch;
            FromNodeId = fromNodeId;
            ToNodeId = toNodeId;
            PressureProvider = pressureProvider;
        }

        public HydraulicBranch Branch { get; }

        public string FromNodeId { get; }

        public string ToNodeId { get; }

        public string DirectedNodePairKey => $"{FromNodeId}\u001f{ToNodeId}";

        public int ParallelBranchCount { get; set; } = 1;

        public PressureProvider PressureProvider { get; }
    }

    private sealed class PressureProvider
    {
        public static PressureProvider None { get; } = new(
            0,
            Array.Empty<Pump>(),
            new Fluid("placeholder", "Placeholder", densityKilogramsPerCubicMeter: 0),
            PumpCalculator.StandardGravityMetersPerSecondSquared);

        private readonly IReadOnlyList<Pump> pumps;
        private readonly Fluid fluid;
        private readonly double gravitationalAccelerationMetersPerSecondSquared;

        public PressureProvider(
            double constantPressureIncreasePascals,
            IReadOnlyList<Pump> pumps,
            Fluid fluid,
            double gravitationalAccelerationMetersPerSecondSquared)
        {
            ConstantPressureIncreasePascals = constantPressureIncreasePascals;
            this.pumps = pumps;
            this.fluid = fluid;
            this.gravitationalAccelerationMetersPerSecondSquared = gravitationalAccelerationMetersPerSecondSquared;
        }

        public double ConstantPressureIncreasePascals { get; }

        public bool HasExternalPressure => ConstantPressureIncreasePascals > 0 || pumps.Count > 0;

        public double CalculatePressureIncreasePascals(double flowRate)
        {
            return ConstantPressureIncreasePascals
                + pumps.Sum(pump => PumpCalculator.CalculatePressureIncreasePascals(
                    pump,
                    fluid,
                    flowRate,
                    gravitationalAccelerationMetersPerSecondSquared));
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
