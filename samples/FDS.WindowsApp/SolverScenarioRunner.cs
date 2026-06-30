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

    public static string FormatResult(
        HydraulicSolverResult result,
        SolverScenarioParameters? parameters = null,
        UnitDisplaySettings? unitSettings = null)
    {
        UnitDisplaySettings units = unitSettings ?? UnitDisplaySettings.Default;
        SolverScenarioReport report = CreateReport(result, parameters, units);
        var builder = new StringBuilder();

        builder.AppendLine("Fluid Dynamics Simulator - Windows-App-Test");
        builder.AppendLine("Szenario: zwei parallele hydraulische Stränge mit bekannter Druckdifferenz");
        if (parameters is not null)
        {
            builder.AppendLine();
            builder.AppendLine("Eingaben:");
            builder.AppendLine(report.InputSummaryText);
        }

        builder.AppendLine();
        builder.AppendLine($"Status: {report.StatusText}");
        builder.AppendLine($"Iterationen: {report.IterationsText}");
        builder.AppendLine($"Finales Knotenbilanz-Residuum: {report.NodeResidualText}");
        builder.AppendLine($"Finales Druck-Residuum: {report.PressureResidualText}");
        builder.AppendLine($"Bewertung: {report.AssessmentText}");
        if (!string.IsNullOrWhiteSpace(report.ReviewSummaryText))
        {
            builder.AppendLine();
            builder.AppendLine("Prüfhinweise:");
            builder.AppendLine(report.ReviewSummaryText);
        }

        builder.AppendLine();

        builder.AppendLine("Strang-Volumenströme:");
        foreach (BranchFlowReportRow volumeFlow in report.BranchFlows)
        {
            builder.AppendLine($"- {volumeFlow.BranchName}: {volumeFlow.VolumeFlowRateText}");
        }

        builder.AppendLine();
        builder.AppendLine("Druckresiduen:");
        foreach (PressureResidualReportRow residual in report.PressureResiduals)
        {
            builder.AppendLine(
                $"- {residual.ElementName}: Residuum {residual.ResidualText}, " +
                $"verfügbar {residual.AvailablePressureText}, " +
                $"erforderlich {residual.RequiredPressureText}");
        }

        builder.AppendLine();
        builder.AppendLine("Letzte Iterationen:");
        foreach (IterationReportRow iteration in report.Iterations.TakeLast(8))
        {
            builder.AppendLine(
                $"- #{iteration.IterationNumberText}: Knotenbilanz-Residuum " +
                $"{iteration.NodeResidualText}, Druck-Residuum {iteration.PressureResidualText}");
        }

        return builder.ToString();
    }

    public static SolverScenarioReport CreateReport(
        HydraulicSolverResult result,
        SolverScenarioParameters? parameters = null,
        UnitDisplaySettings? unitSettings = null)
    {
        ArgumentNullException.ThrowIfNull(result);

        UnitDisplaySettings units = unitSettings ?? UnitDisplaySettings.Default;

        var branchRows = result.SolvedVolumetricFlowRatesCubicMetersPerSecond
            .OrderBy(item => item.Key, StringComparer.Ordinal)
            .Select(item => new BranchFlowReportRow(
                FormatElementName(item.Key),
                units.FormatVolumeFlow(item.Value)))
            .ToList();

        var residualRows = result.PressureResiduals
            .OrderBy(item => item.ElementId, StringComparer.Ordinal)
            .Select(item => new PressureResidualReportRow(
                FormatElementName(item.ElementId),
                units.FormatPressure(item.ResidualPressurePascals),
                units.FormatPressure(item.AvailablePressureIncreasePascals),
                units.FormatPressure(item.RequiredPressureIncreasePascals)))
            .ToList();

        var iterationRows = result.IterationHistory
            .Select(item => new IterationReportRow(
                item.IterationNumber.ToString(CultureInfo.InvariantCulture),
                units.FormatVolumeFlow(item.MaxNodeBalanceResidualCubicMetersPerSecond),
                units.FormatPressure(item.MaxPressureResidualPascals)))
            .ToList();

        return new SolverScenarioReport(
            FormatStatus(result.Status),
            result.Iterations.ToString(CultureInfo.InvariantCulture),
            units.FormatVolumeFlow(result.MaxNodeBalanceResidualCubicMetersPerSecond),
            units.FormatPressure(result.MaxPressureResidualPascals),
            SolverScenarioReview.CreateAssessment(result),
            parameters is null ? string.Empty : FormatInputSummary(parameters, units),
            parameters is null
                ? string.Empty
                : string.Join(Environment.NewLine, SolverScenarioReview.CreateMessages(parameters, result)),
            branchRows,
            residualRows,
            iterationRows);
    }

    public static IReadOnlyList<PresetComparisonReportRow> CreatePresetComparison(UnitDisplaySettings? unitSettings = null)
    {
        UnitDisplaySettings units = unitSettings ?? UnitDisplaySettings.Default;

        return SolverScenarioPreset.All
            .Select(preset => CreatePresetComparisonRow(preset, units))
            .ToList();
    }

    public static string FormatPresetComparison(
        IReadOnlyList<PresetComparisonReportRow>? rows = null,
        UnitDisplaySettings? unitSettings = null)
    {
        rows ??= CreatePresetComparison(unitSettings);

        var builder = new StringBuilder();
        builder.AppendLine("Preset-Vergleich:");

        foreach (PresetComparisonReportRow row in rows)
        {
            builder.AppendLine(
                $"- {row.ScenarioName}: Status {row.StatusText}, " +
                $"Iterationen {row.IterationsText}, " +
                $"Knotenbilanz {row.NodeResidualText}, " +
                $"Druck {row.PressureResidualText}, " +
                $"Strang A {row.BranchAFlowText}, " +
                $"Strang B {row.BranchBFlowText}, " +
                $"Bewertung {row.AssessmentText}");
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

    private static PresetComparisonReportRow CreatePresetComparisonRow(
        SolverScenarioPreset preset,
        UnitDisplaySettings unitSettings)
    {
        HydraulicSolverResult result = RunParallelBranchScenario(preset.Parameters);
        SolverScenarioReport report = CreateReport(result, preset.Parameters, unitSettings);

        return new PresetComparisonReportRow(
            preset.Name,
            report.StatusText,
            report.IterationsText,
            report.NodeResidualText,
            report.PressureResidualText,
            GetBranchFlow(report, "Strang A"),
            GetBranchFlow(report, "Strang B"),
            report.AssessmentText);
    }

    private static string GetBranchFlow(SolverScenarioReport report, string branchName)
    {
        return report.BranchFlows
            .FirstOrDefault(row => row.BranchName == branchName)
            ?.VolumeFlowRateText
            ?? "-";
    }

    private static string FormatInputSummary(
        SolverScenarioParameters parameters,
        UnitDisplaySettings unitSettings)
    {
        return string.Join(
            Environment.NewLine,
            $"- Druckdifferenz: {unitSettings.FormatPressure(parameters.PressureDifferencePascals)}",
            $"- Rohrdurchmesser: {unitSettings.FormatLength(parameters.PipeInnerDiameterMeters)}",
            $"- Zeta Strang A: {FormatNumber(parameters.BranchAZeta)}",
            $"- Zeta Strang B: {FormatNumber(parameters.BranchBZeta)}",
            $"- Gesamtvolumenstrom: {unitSettings.FormatVolumeFlow(parameters.TotalVolumeFlowRateCubicMetersPerSecond)}");
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("G6", CultureInfo.InvariantCulture);
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
