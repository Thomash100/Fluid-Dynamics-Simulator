using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Associates a hydraulic branch with a known nonnegative volumetric flow rate.
/// This is input data for fixed network evaluation, not a solved flow.
/// </summary>
public sealed class HydraulicBranchFlow
{
    public HydraulicBranchFlow(HydraulicBranch branch, double volumetricFlowRateCubicMetersPerSecond)
    {
        Branch = branch ?? throw new ArgumentNullException(nameof(branch));
        VolumetricFlowRateCubicMetersPerSecond = HydraulicValidation.EnsureNonNegativeFinite(
            volumetricFlowRateCubicMetersPerSecond,
            nameof(volumetricFlowRateCubicMetersPerSecond));
    }

    public HydraulicBranch Branch { get; }

    public string BranchId => Branch.Id;

    public double VolumetricFlowRateCubicMetersPerSecond { get; }
}
