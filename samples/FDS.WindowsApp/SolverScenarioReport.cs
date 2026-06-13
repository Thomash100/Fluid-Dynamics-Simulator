namespace FDS.WindowsApp;

internal sealed class SolverScenarioReport
{
    public SolverScenarioReport(
        string statusText,
        string iterationsText,
        string nodeResidualText,
        string pressureResidualText,
        string assessmentText,
        string inputSummaryText,
        string reviewSummaryText,
        IReadOnlyList<BranchFlowReportRow> branchFlows,
        IReadOnlyList<PressureResidualReportRow> pressureResiduals,
        IReadOnlyList<IterationReportRow> iterations)
    {
        StatusText = statusText;
        IterationsText = iterationsText;
        NodeResidualText = nodeResidualText;
        PressureResidualText = pressureResidualText;
        AssessmentText = assessmentText;
        InputSummaryText = inputSummaryText;
        ReviewSummaryText = reviewSummaryText;
        BranchFlows = branchFlows;
        PressureResiduals = pressureResiduals;
        Iterations = iterations;
    }

    public string StatusText { get; }

    public string IterationsText { get; }

    public string NodeResidualText { get; }

    public string PressureResidualText { get; }

    public string AssessmentText { get; }

    public string InputSummaryText { get; }

    public string ReviewSummaryText { get; }

    public IReadOnlyList<BranchFlowReportRow> BranchFlows { get; }

    public IReadOnlyList<PressureResidualReportRow> PressureResiduals { get; }

    public IReadOnlyList<IterationReportRow> Iterations { get; }
}

internal sealed class BranchFlowReportRow
{
    public BranchFlowReportRow(string branchName, string volumeFlowRateText)
    {
        BranchName = branchName;
        VolumeFlowRateText = volumeFlowRateText;
    }

    public string BranchName { get; }

    public string VolumeFlowRateText { get; }
}

internal sealed class PressureResidualReportRow
{
    public PressureResidualReportRow(
        string elementName,
        string residualText,
        string availablePressureText,
        string requiredPressureText)
    {
        ElementName = elementName;
        ResidualText = residualText;
        AvailablePressureText = availablePressureText;
        RequiredPressureText = requiredPressureText;
    }

    public string ElementName { get; }

    public string ResidualText { get; }

    public string AvailablePressureText { get; }

    public string RequiredPressureText { get; }
}

internal sealed class IterationReportRow
{
    public IterationReportRow(
        string iterationNumberText,
        string nodeResidualText,
        string pressureResidualText)
    {
        IterationNumberText = iterationNumberText;
        NodeResidualText = nodeResidualText;
        PressureResidualText = pressureResidualText;
    }

    public string IterationNumberText { get; }

    public string NodeResidualText { get; }

    public string PressureResidualText { get; }
}
