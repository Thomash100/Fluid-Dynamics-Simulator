using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests;

public sealed class HydraulicBoundaryConditionTests
{
    [Fact]
    public void SourceFlow_StoresNodeAndFlow()
    {
        var boundary = HydraulicBoundaryCondition.SourceFlow("source-1", "node-a", 0.01);

        Assert.Equal("source-1", boundary.Id);
        Assert.Equal(HydraulicBoundaryConditionKind.SourceFlow, boundary.Kind);
        Assert.Equal("node-a", boundary.NodeId);
        Assert.Equal(0.01, boundary.VolumetricFlowRateCubicMetersPerSecond);
    }

    [Fact]
    public void SinkFlow_RejectsNegativeFlow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => HydraulicBoundaryCondition.SinkFlow("sink-1", "node-b", -0.01));
    }

    [Fact]
    public void KnownPressure_StoresPressure()
    {
        var boundary = HydraulicBoundaryCondition.KnownPressure("pressure-1", "node-a", 200000);

        Assert.Equal(HydraulicBoundaryConditionKind.KnownPressure, boundary.Kind);
        Assert.Equal("node-a", boundary.NodeId);
        Assert.Equal(200000, boundary.PressurePascals);
    }

    [Fact]
    public void KnownPressureDifference_StoresNodePairAndDifference()
    {
        var boundary = HydraulicBoundaryCondition.KnownPressureDifference("dp-1", "node-a", "node-b", 5000);

        Assert.Equal(HydraulicBoundaryConditionKind.KnownPressureDifference, boundary.Kind);
        Assert.Equal("node-a", boundary.FromNodeId);
        Assert.Equal("node-b", boundary.ToNodeId);
        Assert.Equal(5000, boundary.PressureDifferencePascals);
    }

    [Fact]
    public void PumpCurve_StoresPumpBoundary()
    {
        var pump = new Pump(
            "pump-1",
            "Pump",
            new PumpCurve(new[]
            {
                new PumpCurvePoint(0, 5),
                new PumpCurvePoint(0.02, 4)
            }));

        var boundary = HydraulicBoundaryCondition.PumpCurve("pump-boundary-1", "node-a", "node-b", pump);

        Assert.Equal(HydraulicBoundaryConditionKind.PumpCurve, boundary.Kind);
        Assert.Equal("node-a", boundary.FromNodeId);
        Assert.Equal("node-b", boundary.ToNodeId);
        Assert.Same(pump, boundary.Pump);
    }
}
