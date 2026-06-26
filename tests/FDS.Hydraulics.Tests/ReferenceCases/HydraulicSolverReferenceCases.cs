using FDS.Core.Models;
using FDS.Hydraulics.Calculations;
using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests.ReferenceCases;

internal static class HydraulicSolverReferenceCases
{
    private const double PipeDiameterMeters = 0.1;
    private const double DynamicViscosityPascalSeconds = 0.001;
    private const double ReferencePressureDifferencePascals = 2000;
    private const double WaterDensityKilogramsPerCubicMeter = 1000;

    private static readonly Fluid Water = new(
        "water",
        "Water",
        densityKilogramsPerCubicMeter: WaterDensityKilogramsPerCubicMeter);

    public static HydraulicSolverReferenceCase SingleBranchKnownPressureDifference()
    {
        var branch = CreateBranch("branch-1", "source", "sink", zeta: 1);
        var flow = CreateFlowForVelocity(velocityMetersPerSecond: 2);
        var input = CreateInput(
            new[] { branch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", flow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", flow),
                HydraulicBoundaryCondition.KnownPressureDifference(
                    "known-dp",
                    "source",
                    "sink",
                    ReferencePressureDifferencePascals)
            });

        return new HydraulicSolverReferenceCase(
            "single-branch-known-pressure-difference",
            "Einstrangnetz mit bekannter Druckdifferenz.",
            input,
            HydraulicSolverStatus.Converged,
            new Dictionary<string, double>
            {
                ["branch-1"] = flow
            },
            BalancedSourceSinkResiduals(),
            new Dictionary<string, ReferencePressureResidual>
            {
                ["branch-1"] = BalancedPressureResidual()
            },
            ExpectedMaxNodeResidualCubicMetersPerSecond: 0,
            ExpectedMaxPressureResidualPascals: 0);
    }

    public static HydraulicSolverReferenceCase TwoParallelBranches()
    {
        var lowResistanceBranch = CreateBranch("branch-low", "source", "sink", zeta: 1);
        var highResistanceBranch = CreateBranch("branch-high", "source", "sink", zeta: 4);
        var lowResistanceFlow = CreateFlowForVelocity(velocityMetersPerSecond: 2);
        var highResistanceFlow = CreateFlowForVelocity(velocityMetersPerSecond: 1);
        var totalFlow = lowResistanceFlow + highResistanceFlow;
        var input = CreateInput(
            new[] { lowResistanceBranch, highResistanceBranch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", totalFlow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", totalFlow),
                HydraulicBoundaryCondition.KnownPressureDifference(
                    "known-dp",
                    "source",
                    "sink",
                    ReferencePressureDifferencePascals)
            });

        return new HydraulicSolverReferenceCase(
            "two-parallel-branches",
            "Zwei parallele Stränge mit gleicher Druckdifferenz und bekannter Gesamtmenge.",
            input,
            HydraulicSolverStatus.Converged,
            new Dictionary<string, double>
            {
                ["branch-low"] = lowResistanceFlow,
                ["branch-high"] = highResistanceFlow
            },
            BalancedSourceSinkResiduals(),
            new Dictionary<string, ReferencePressureResidual>
            {
                ["branch-low"] = BalancedPressureResidual(),
                ["branch-high"] = BalancedPressureResidual()
            },
            ExpectedMaxNodeResidualCubicMetersPerSecond: 0,
            ExpectedMaxPressureResidualPascals: 0);
    }

    public static HydraulicSolverReferenceCase FixedPumpPressureIncrease()
    {
        var branch = CreateBranch("branch-1", "source", "sink", zeta: 1);
        var flow = CreateFlowForVelocity(velocityMetersPerSecond: 2);
        var pump = CreateFixedPressurePump("pump-1", ReferencePressureDifferencePascals);
        var input = CreateInput(
            new[] { branch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", flow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", flow),
                HydraulicBoundaryCondition.PumpCurve("pump-boundary", "source", "sink", pump)
            });

        return new HydraulicSolverReferenceCase(
            "fixed-pump-pressure-increase",
            "Feste Pumpendruckerhöhung über eine konstante Pumpenkennlinie.",
            input,
            HydraulicSolverStatus.Converged,
            new Dictionary<string, double>
            {
                ["branch-1"] = flow
            },
            BalancedSourceSinkResiduals(),
            new Dictionary<string, ReferencePressureResidual>
            {
                ["branch-1"] = BalancedPressureResidual()
            },
            ExpectedMaxNodeResidualCubicMetersPerSecond: 0,
            ExpectedMaxPressureResidualPascals: 0);
    }

    public static HydraulicSolverReferenceCase MaxIterationsReached()
    {
        var branch = CreateBranch("branch-1", "source", "sink", zeta: 1);
        var flow = CreateFlowForVelocity(velocityMetersPerSecond: 2);
        var input = CreateInput(
            new[] { branch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", flow),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", flow),
                HydraulicBoundaryCondition.KnownPressureDifference(
                    "known-dp",
                    "source",
                    "sink",
                    ReferencePressureDifferencePascals)
            },
            new HydraulicSolverOptions(
                maxIterations: 1,
                flowResidualToleranceCubicMetersPerSecond: 1e-12,
                pressureResidualTolerancePascals: 1e-9,
                relaxationFactor: 0.1));

        return new HydraulicSolverReferenceCase(
            "max-iterations-reached",
            "Zu kleine Iterationszahl bei bewusst strengen Toleranzen.",
            input,
            HydraulicSolverStatus.MaxIterationsReached,
            new Dictionary<string, double>(),
            new Dictionary<string, double>(),
            new Dictionary<string, ReferencePressureResidual>(),
            ExpectedIterations: 1,
            MinimumMaxNodeResidualCubicMetersPerSecond: 0,
            MinimumMaxPressureResidualPascals: 0);
    }

    public static HydraulicSolverReferenceCase InvalidInput()
    {
        var branch = new HydraulicBranch(
            "branch-1",
            "Invalid branch",
            new[] { new Pipe("pipe-1", lengthMeters: 0, innerDiameterMeters: PipeDiameterMeters) },
            localResistances: new[] { new LocalResistance("res-1", "Resistance", zeta: 1) });
        var input = CreateInput(
            new[] { branch },
            Array.Empty<HydraulicBoundaryCondition>());

        return new HydraulicSolverReferenceCase(
            "invalid-input",
            "Ungültiger Branch ohne Topologie-Referenzen.",
            input,
            HydraulicSolverStatus.InvalidInput,
            new Dictionary<string, double>(),
            new Dictionary<string, double>(),
            new Dictionary<string, ReferencePressureResidual>(),
            ExpectedIterations: 0,
            ExpectedMaxNodeResidualCubicMetersPerSecond: 0,
            ExpectedMaxPressureResidualPascals: 0,
            ExpectIterationHistory: false);
    }

    public static HydraulicSolverReferenceCase ZeroFlowBoundaryCase()
    {
        var branch = CreateBranch("branch-1", "source", "sink", zeta: 1);
        var pump = CreateFixedPressurePump("pump-1", pressureIncreasePascals: 0);
        var input = CreateInput(
            new[] { branch },
            new[]
            {
                HydraulicBoundaryCondition.SourceFlow("source-flow", "source", 0),
                HydraulicBoundaryCondition.SinkFlow("sink-flow", "sink", 0),
                HydraulicBoundaryCondition.PumpCurve("pump-boundary", "source", "sink", pump)
            });

        return new HydraulicSolverReferenceCase(
            "zero-flow-boundary-case",
            "Nullfluss-Grenzfall mit druckneutraler Pumpenrandbedingung.",
            input,
            HydraulicSolverStatus.Converged,
            new Dictionary<string, double>
            {
                ["branch-1"] = 0
            },
            BalancedSourceSinkResiduals(),
            new Dictionary<string, ReferencePressureResidual>
            {
                ["branch-1"] = new(0, 0, 0)
            },
            ExpectedIterations: 0,
            ExpectedMaxNodeResidualCubicMetersPerSecond: 0,
            ExpectedMaxPressureResidualPascals: 0);
    }

    private static HydraulicSolverInput CreateInput(
        IEnumerable<HydraulicBranch> branches,
        IEnumerable<HydraulicBoundaryCondition> boundaryConditions,
        HydraulicSolverOptions? options = null)
    {
        var topology = new Network(
            "network-1",
            new[]
            {
                new Node("source"),
                new Node("sink")
            },
            Array.Empty<Edge>(),
            Water);

        return new HydraulicSolverInput(
            topology,
            branches,
            Water,
            DynamicViscosityPascalSeconds,
            boundaryConditions,
            options);
    }

    private static HydraulicBranch CreateBranch(
        string id,
        string fromNodeId,
        string toNodeId,
        double zeta)
    {
        return new HydraulicBranch(
            id,
            id,
            new[]
            {
                new Pipe(
                    $"{id}-pipe",
                    lengthMeters: 0,
                    innerDiameterMeters: PipeDiameterMeters,
                    fromNodeId,
                    toNodeId)
            },
            localResistances: new[] { new LocalResistance($"{id}-res", $"{id} resistance", zeta) });
    }

    private static Pump CreateFixedPressurePump(string id, double pressureIncreasePascals)
    {
        var fixedHeadMeters = pressureIncreasePascals
            / (WaterDensityKilogramsPerCubicMeter * PumpCalculator.StandardGravityMetersPerSecondSquared);

        return new Pump(
            id,
            "Fixed pressure pump",
            new PumpCurve(new[]
            {
                new PumpCurvePoint(0, fixedHeadMeters),
                new PumpCurvePoint(0.1, fixedHeadMeters)
            }));
    }

    private static double CreateFlowForVelocity(double velocityMetersPerSecond)
    {
        var pipe = new Pipe("reference-pipe", lengthMeters: 0, innerDiameterMeters: PipeDiameterMeters);

        return PipeFlowCalculator.CalculateCrossSectionalAreaSquareMeters(pipe)
            * velocityMetersPerSecond;
    }

    private static Dictionary<string, double> BalancedSourceSinkResiduals()
    {
        return new Dictionary<string, double>
        {
            ["sink"] = 0,
            ["source"] = 0
        };
    }

    private static ReferencePressureResidual BalancedPressureResidual()
    {
        return new ReferencePressureResidual(
            ReferencePressureDifferencePascals,
            ReferencePressureDifferencePascals,
            0);
    }
}
