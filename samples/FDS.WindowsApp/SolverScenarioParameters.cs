namespace FDS.WindowsApp;

internal sealed class SolverScenarioParameters
{
    public const double DefaultPressureDifferencePascals = 2000.0;
    public const double DefaultPipeInnerDiameterMeters = 0.1;
    public const double DefaultBranchAZeta = 1.0;
    public const double DefaultBranchBZeta = 4.0;

    public double PressureDifferencePascals { get; init; } = DefaultPressureDifferencePascals;

    public double PipeInnerDiameterMeters { get; init; } = DefaultPipeInnerDiameterMeters;

    public double BranchAZeta { get; init; } = DefaultBranchAZeta;

    public double BranchBZeta { get; init; } = DefaultBranchBZeta;

    public double TotalVolumeFlowRateCubicMetersPerSecond { get; init; } =
        CalculateCircularArea(DefaultPipeInnerDiameterMeters) * 3.0;

    public static SolverScenarioParameters Default { get; } = new();

    private static double CalculateCircularArea(double diameterMeters)
    {
        return Math.PI * Math.Pow(diameterMeters, 2.0) / 4.0;
    }
}
