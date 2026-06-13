using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

public sealed class HydraulicSolverOptions
{
    public HydraulicSolverOptions(
        int maxIterations = 50,
        double flowResidualToleranceCubicMetersPerSecond = 1e-8,
        double pressureResidualTolerancePascals = 1,
        double relaxationFactor = 1)
    {
        if (maxIterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxIterations), maxIterations, "Max iterations must be greater than zero.");
        }

        MaxIterations = maxIterations;
        FlowResidualToleranceCubicMetersPerSecond = HydraulicValidation.EnsurePositiveFinite(
            flowResidualToleranceCubicMetersPerSecond,
            nameof(flowResidualToleranceCubicMetersPerSecond));
        PressureResidualTolerancePascals = HydraulicValidation.EnsurePositiveFinite(
            pressureResidualTolerancePascals,
            nameof(pressureResidualTolerancePascals));
        RelaxationFactor = HydraulicValidation.EnsurePositiveFraction(relaxationFactor, nameof(relaxationFactor));
    }

    public int MaxIterations { get; }

    public double FlowResidualToleranceCubicMetersPerSecond { get; }

    public double PressureResidualTolerancePascals { get; }

    public double RelaxationFactor { get; }

    public static HydraulicSolverOptions Default { get; } = new();
}
