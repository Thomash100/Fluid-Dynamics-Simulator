using FDS.Hydraulics.Models;

namespace FDS.Hydraulics.Tests.ReferenceCases;

internal sealed record HydraulicSolverReferenceCase(
    string Id,
    string Description,
    HydraulicSolverInput Input,
    HydraulicSolverStatus ExpectedStatus,
    IReadOnlyDictionary<string, double> ExpectedBranchFlowsCubicMetersPerSecond,
    IReadOnlyDictionary<string, double> ExpectedNodeResidualsCubicMetersPerSecond,
    IReadOnlyDictionary<string, ReferencePressureResidual> ExpectedPressureResidualsPascals,
    int? ExpectedIterations = null,
    double? ExpectedMaxNodeResidualCubicMetersPerSecond = null,
    double? ExpectedMaxPressureResidualPascals = null,
    double? MinimumMaxNodeResidualCubicMetersPerSecond = null,
    double? MinimumMaxPressureResidualPascals = null,
    bool ExpectIterationHistory = true);

internal sealed record ReferencePressureResidual(
    double AvailablePressureIncreasePascals,
    double RequiredPressureIncreasePascals,
    double ResidualPressurePascals);
