using System.Globalization;
using System.Text;
using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.WindowsApp;

internal static class SolverScenarioRunner
{
    private const double WaterDynamicViscosityPascalSeconds = 0.001;

    public static HydraulicSolverResult RunParallelBranchScenario()
    {
        return RunParallelBranchScenario(SolverScenarioParameters.Default);
    }

    public static HydraulicSolverResult RunParallelBranchScenario(SolverScenarioParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var water = new Fluid("water", "Water", densityKilogramsPerCubicMeter: 1000);

        var sourceNode = new Node("source", "Source");
        var sinkNode = new Node("sink", "Sink");
        var network = new Network("windows-app-test-network", [sourceNode, sinkNode], []);

        HydraulicBranch branchA = CreateBranch(
            "branch-a",
            parameters.PipeInnerDiameterMeters,
            parameters.BranchAZeta);
        HydraulicBranch branchB = CreateBranch(
            "branch-b",
            parameters.PipeInnerDiameterMeters,
            parameters.BranchBZeta);

        var input = new HydraulicSolverInput(
            network,
            [branchA, branchB],
            water,
            WaterDynamicViscosityPascalSeconds,
            [
                HydraulicBoundaryCondition.SourceFlow(
                    "source-flow",
                    sourceNode.Id,
                    parameters.TotalVolumeFlowRateCubicMetersPerSecond),
                HydraulicBoundaryCondition.SinkFlow(
                    "sink-flow",
                    sinkNode.Id,
                    parameters.TotalVolumeFlowRateCubicMetersPerSecond),
                HydraulicBoundaryCondition.KnownPressureDifference(
                    "dp-source-sink",
                    sourceNode.Id,
                    sinkNode.Id,
                    parameters.PressureDifferencePascals),
            ],
            new HydraulicSolverOptions(
                maxIterations: 60,
                flowResidualToleranceCubicMetersPerSecond: 1e-8,
                pressureResidualTolerancePascals: 0.25));

        return new SmallHydraulicNetworkSolver().Solve(input);
    }

    public static string FormatResult(HydraulicSolverResult result, SolverScenarioParameters? parameters = null)
    {
        var builder = new StringBuilder();
        IFormatProvider culture = CultureInfo.InvariantCulture;

        builder.AppendLine("Fluid Dynamics Simulator - Windows App Test");
        builder.AppendLine("Scenario: two parallel hydraulic branches with known pressure difference");
        if (parameters is not null)
        {
            builder.AppendLine();
            builder.AppendLine("Input:");
            builder.AppendLine($"- Pressure difference: {parameters.PressureDifferencePascals.ToString("G6", culture)} Pa");
            builder.AppendLine($"- Pipe diameter: {parameters.PipeInnerDiameterMeters.ToString("G6", culture)} m");
            builder.AppendLine($"- Branch A zeta: {parameters.BranchAZeta.ToString("G6", culture)}");
            builder.AppendLine($"- Branch B zeta: {parameters.BranchBZeta.ToString("G6", culture)}");
            builder.AppendLine($"- Total flow: {parameters.TotalVolumeFlowRateCubicMetersPerSecond.ToString("G6", culture)} m3/s");
        }

        builder.AppendLine();
        builder.AppendLine($"Status: {result.Status}");
        builder.AppendLine($"Iterations: {result.Iterations}");
        builder.AppendLine($"Final node residual: {result.MaxNodeBalanceResidualCubicMetersPerSecond.ToString("G6", culture)} m3/s");
        builder.AppendLine($"Final pressure residual: {result.MaxPressureResidualPascals.ToString("G6", culture)} Pa");
        builder.AppendLine();

        builder.AppendLine("Branch volume flows:");
        foreach (KeyValuePair<string, double> volumeFlow in result.SolvedVolumetricFlowRatesCubicMetersPerSecond.OrderBy(item => item.Key))
        {
            builder.AppendLine($"- {volumeFlow.Key}: {volumeFlow.Value.ToString("G6", culture)} m3/s");
        }

        builder.AppendLine();
        builder.AppendLine("Pressure residuals:");
        foreach (HydraulicPressureResidual residual in result.PressureResiduals.OrderBy(item => item.ElementId))
        {
            builder.AppendLine(
                $"- {residual.ElementId}: residual {residual.ResidualPressurePascals.ToString("G6", culture)} Pa, " +
                $"available {residual.AvailablePressureIncreasePascals.ToString("G6", culture)} Pa, " +
                $"required {residual.RequiredPressureIncreasePascals.ToString("G6", culture)} Pa");
        }

        builder.AppendLine();
        builder.AppendLine("Last iterations:");
        foreach (HydraulicSolverIteration iteration in result.IterationHistory.TakeLast(8))
        {
            builder.AppendLine(
                $"- #{iteration.IterationNumber}: node residual " +
                $"{iteration.MaxNodeBalanceResidualCubicMetersPerSecond.ToString("G6", culture)} m3/s, " +
                $"pressure residual {iteration.MaxPressureResidualPascals.ToString("G6", culture)} Pa");
        }

        return builder.ToString();
    }

    private static HydraulicBranch CreateBranch(
        string branchId,
        double diameterMeters,
        double localResistanceCoefficient)
    {
        var pipe = new Pipe(
            $"{branchId}-pipe",
            lengthMeters: 0.0,
            innerDiameterMeters: diameterMeters,
            fromNodeId: "source",
            toNodeId: "sink");

        var resistance = new LocalResistance(
            $"{branchId}-resistance",
            $"{branchId} resistance",
            localResistanceCoefficient);

        return new HydraulicBranch(
            branchId,
            branchId,
            [pipe],
            localResistances: [resistance]);
    }
}
