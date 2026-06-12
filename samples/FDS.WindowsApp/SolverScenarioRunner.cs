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

        var sourceNode = new Node("source", "Quelle");
        var sinkNode = new Node("sink", "Senke");
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

        builder.AppendLine("Fluid Dynamics Simulator - Windows-App-Test");
        builder.AppendLine("Szenario: zwei parallele hydraulische Stränge mit bekannter Druckdifferenz");
        if (parameters is not null)
        {
            builder.AppendLine();
            builder.AppendLine("Eingaben:");
            builder.AppendLine($"- Druckdifferenz: {parameters.PressureDifferencePascals.ToString("G6", culture)} Pa");
            builder.AppendLine($"- Rohrdurchmesser: {parameters.PipeInnerDiameterMeters.ToString("G6", culture)} m");
            builder.AppendLine($"- Zeta Strang A: {parameters.BranchAZeta.ToString("G6", culture)}");
            builder.AppendLine($"- Zeta Strang B: {parameters.BranchBZeta.ToString("G6", culture)}");
            builder.AppendLine($"- Gesamtvolumenstrom: {parameters.TotalVolumeFlowRateCubicMetersPerSecond.ToString("G6", culture)} m3/s");
        }

        builder.AppendLine();
        builder.AppendLine($"Status: {FormatStatus(result.Status)}");
        builder.AppendLine($"Iterationen: {result.Iterations}");
        builder.AppendLine($"Finales Knotenbilanz-Residuum: {result.MaxNodeBalanceResidualCubicMetersPerSecond.ToString("G6", culture)} m3/s");
        builder.AppendLine($"Finales Druck-Residuum: {result.MaxPressureResidualPascals.ToString("G6", culture)} Pa");
        builder.AppendLine();

        builder.AppendLine("Strang-Volumenströme:");
        foreach (KeyValuePair<string, double> volumeFlow in result.SolvedVolumetricFlowRatesCubicMetersPerSecond.OrderBy(item => item.Key))
        {
            builder.AppendLine($"- {FormatElementName(volumeFlow.Key)}: {volumeFlow.Value.ToString("G6", culture)} m3/s");
        }

        builder.AppendLine();
        builder.AppendLine("Druckresiduen:");
        foreach (HydraulicPressureResidual residual in result.PressureResiduals.OrderBy(item => item.ElementId))
        {
            builder.AppendLine(
                $"- {FormatElementName(residual.ElementId)}: Residuum {residual.ResidualPressurePascals.ToString("G6", culture)} Pa, " +
                $"verfügbar {residual.AvailablePressureIncreasePascals.ToString("G6", culture)} Pa, " +
                $"erforderlich {residual.RequiredPressureIncreasePascals.ToString("G6", culture)} Pa");
        }

        builder.AppendLine();
        builder.AppendLine("Letzte Iterationen:");
        foreach (HydraulicSolverIteration iteration in result.IterationHistory.TakeLast(8))
        {
            builder.AppendLine(
                $"- #{iteration.IterationNumber}: Knotenbilanz-Residuum " +
                $"{iteration.MaxNodeBalanceResidualCubicMetersPerSecond.ToString("G6", culture)} m3/s, " +
                $"Druck-Residuum {iteration.MaxPressureResidualPascals.ToString("G6", culture)} Pa");
        }

        return builder.ToString();
    }

    public static string FormatStatus(HydraulicSolverStatus status)
    {
        return status switch
        {
            HydraulicSolverStatus.Converged => "konvergiert",
            HydraulicSolverStatus.MaxIterationsReached => "maximale Iterationszahl erreicht",
            HydraulicSolverStatus.InvalidInput => "ungültige Eingabe",
            _ => status.ToString(),
        };
    }

    private static string FormatElementName(string elementId)
    {
        return elementId switch
        {
            "branch-a" => "Strang A",
            "branch-b" => "Strang B",
            _ => elementId,
        };
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
