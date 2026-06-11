using System.Collections.ObjectModel;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Fixed network evaluation result across multiple known branch flows.
/// </summary>
public sealed class HydraulicNetworkResult
{
    public HydraulicNetworkResult(IEnumerable<HydraulicNetworkBranchResult> branchResults)
    {
        ArgumentNullException.ThrowIfNull(branchResults);

        var results = branchResults
            .Select(result => result ?? throw new ArgumentException("Network branch results cannot contain null.", nameof(branchResults)))
            .ToList();

        if (results.Count == 0)
        {
            throw new ArgumentException("Network result requires at least one branch result.", nameof(branchResults));
        }

        BranchResults = new ReadOnlyCollection<HydraulicNetworkBranchResult>(results);
        CriticalBranchResult = results
            .OrderByDescending(result => result.RequiredPumpPressureIncreasePascals)
            .First();
    }

    public IReadOnlyList<HydraulicNetworkBranchResult> BranchResults { get; }

    public HydraulicNetworkBranchResult CriticalBranchResult { get; }

    public double RequiredPumpPressureIncreasePascals =>
        CriticalBranchResult.RequiredPumpPressureIncreasePascals;

    public double? RequiredPumpHeadMeters => CriticalBranchResult.RequiredPumpHeadMeters;
}
