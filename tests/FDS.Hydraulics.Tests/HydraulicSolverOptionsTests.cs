using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class HydraulicSolverOptionsTests
{
    [Fact]
    public void Constructor_StoresSolverPreparationSettings()
    {
        var options = new HydraulicSolverOptions(
            maxIterations: 25,
            flowResidualToleranceCubicMetersPerSecond: 1e-6,
            pressureResidualTolerancePascals: 5,
            relaxationFactor: 0.5);

        Assert.Equal(25, options.MaxIterations);
        Assert.Equal(1e-6, options.FlowResidualToleranceCubicMetersPerSecond);
        Assert.Equal(5, options.PressureResidualTolerancePascals);
        Assert.Equal(0.5, options.RelaxationFactor);
    }

    [Fact]
    public void Constructor_RejectsInvalidMaxIterations()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new HydraulicSolverOptions(maxIterations: 0));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1.01)]
    public void Constructor_RejectsInvalidRelaxationFactor(double relaxationFactor)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new HydraulicSolverOptions(relaxationFactor: relaxationFactor));
    }
}
