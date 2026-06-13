namespace FDS.WindowsApp;

internal sealed class SolverScenarioPreset
{
    private SolverScenarioPreset(string name, string description, SolverScenarioParameters parameters)
    {
        Name = name;
        Description = description;
        Parameters = parameters;
    }

    public string Name { get; }

    public string Description { get; }

    public SolverScenarioParameters Parameters { get; }

    public static IReadOnlyList<SolverScenarioPreset> All { get; } =
    [
        new SolverScenarioPreset(
            "Referenzfall",
            "Konvergenter Standardfall mit zwei parallelen Strängen.",
            SolverScenarioParameters.Default),
        new SolverScenarioPreset(
            "Höhere Druckdifferenz",
            "Erhöhte verfügbare Druckdifferenz bei gleichem Gesamtvolumenstrom.",
            new SolverScenarioParameters
            {
                PressureDifferencePascals = 3500,
                PipeInnerDiameterMeters = SolverScenarioParameters.DefaultPipeInnerDiameterMeters,
                BranchAZeta = SolverScenarioParameters.DefaultBranchAZeta,
                BranchBZeta = SolverScenarioParameters.DefaultBranchBZeta,
                TotalVolumeFlowRateCubicMetersPerSecond =
                    SolverScenarioParameters.Default.TotalVolumeFlowRateCubicMetersPerSecond,
            }),
        new SolverScenarioPreset(
            "Engeres Rohr",
            "Kleinerer Rohrdurchmesser zur sichtbaren Veränderung der Druckresiduen.",
            new SolverScenarioParameters
            {
                PressureDifferencePascals = SolverScenarioParameters.DefaultPressureDifferencePascals,
                PipeInnerDiameterMeters = 0.075,
                BranchAZeta = SolverScenarioParameters.DefaultBranchAZeta,
                BranchBZeta = SolverScenarioParameters.DefaultBranchBZeta,
                TotalVolumeFlowRateCubicMetersPerSecond =
                    SolverScenarioParameters.Default.TotalVolumeFlowRateCubicMetersPerSecond,
            }),
    ];

    public override string ToString()
    {
        return Name;
    }
}
