using System.Collections.ObjectModel;

using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Fixed hydraulic network input made of already defined branches and known
/// branch flow rates. It does not solve or distribute flow.
/// </summary>
public sealed class HydraulicNetwork
{
    public HydraulicNetwork(string id, string name, IEnumerable<HydraulicBranchFlow> branchFlows)
    {
        Id = HydraulicValidation.RequiredId(id, nameof(id));
        Name = HydraulicValidation.RequiredName(name, nameof(name));
        BranchFlows = ToReadOnlyList(branchFlows, nameof(branchFlows));

        if (BranchFlows.Count == 0)
        {
            throw new ArgumentException("A hydraulic network requires at least one branch.", nameof(branchFlows));
        }

        ValidateUniqueBranchIds(BranchFlows);
    }

    public string Id { get; }

    public string Name { get; }

    public IReadOnlyList<HydraulicBranchFlow> BranchFlows { get; }

    private static IReadOnlyList<HydraulicBranchFlow> ToReadOnlyList(
        IEnumerable<HydraulicBranchFlow> branchFlows,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(branchFlows);

        var items = branchFlows
            .Select(branchFlow => branchFlow ?? throw new ArgumentException("Branch flow collection cannot contain null.", parameterName))
            .ToList();

        return new ReadOnlyCollection<HydraulicBranchFlow>(items);
    }

    private static void ValidateUniqueBranchIds(IReadOnlyList<HydraulicBranchFlow> branchFlows)
    {
        var duplicateBranchId = branchFlows
            .GroupBy(branchFlow => branchFlow.BranchId, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1)
            ?.Key;

        if (duplicateBranchId is not null)
        {
            throw new ArgumentException($"Branch IDs must be unique. Duplicate ID: {duplicateBranchId}", nameof(branchFlows));
        }
    }
}
