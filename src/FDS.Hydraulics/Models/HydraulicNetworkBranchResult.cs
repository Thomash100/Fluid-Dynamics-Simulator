using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Network-level evaluation for one branch. Required pump pressure is based on
/// the branch loss sum at the known flow rate.
/// </summary>
public sealed class HydraulicNetworkBranchResult
{
    public HydraulicNetworkBranchResult(
        HydraulicBranch branch,
        HydraulicBranchResult branchResult,
        double requiredPumpPressureIncreasePascals,
        double? requiredPumpHeadMeters)
    {
        Branch = branch ?? throw new ArgumentNullException(nameof(branch));
        BranchResult = branchResult ?? throw new ArgumentNullException(nameof(branchResult));
        RequiredPumpPressureIncreasePascals = HydraulicValidation.EnsureNonNegativeFinite(
            requiredPumpPressureIncreasePascals,
            nameof(requiredPumpPressureIncreasePascals));

        if (requiredPumpHeadMeters.HasValue)
        {
            HydraulicValidation.EnsureNonNegativeFinite(requiredPumpHeadMeters.Value, nameof(requiredPumpHeadMeters));
        }

        RequiredPumpHeadMeters = requiredPumpHeadMeters;
    }

    public HydraulicBranch Branch { get; }

    public string BranchId => Branch.Id;

    public HydraulicBranchResult BranchResult { get; }

    public double TotalPressureLossPascals =>
        BranchResult.PipePressureLossPascals + BranchResult.LocalPressureLossPascals;

    public double RequiredPumpPressureIncreasePascals { get; }

    public double? RequiredPumpHeadMeters { get; }
}
