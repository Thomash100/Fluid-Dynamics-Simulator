using System.Collections.ObjectModel;

namespace FDS.Core.Models;

/// <summary>
/// A validated network topology containing nodes, edges, and an optional
/// default fluid. This class does not perform hydraulic or thermal solving.
/// </summary>
public sealed class Network
{
    public Network(string id, IEnumerable<Node> nodes, IEnumerable<Edge> edges, Fluid? fluid = null)
    {
        Id = Validation.RequiredId(id, nameof(id));

        ArgumentNullException.ThrowIfNull(nodes);
        ArgumentNullException.ThrowIfNull(edges);

        var nodeList = nodes.ToList();
        var edgeList = edges.ToList();

        ValidateUniqueModelIds(nodeList, edgeList);
        ValidateEdgeReferences(nodeList, edgeList);

        Nodes = new ReadOnlyDictionary<string, Node>(nodeList.ToDictionary(node => node.Id, StringComparer.Ordinal));
        Edges = new ReadOnlyDictionary<string, Edge>(edgeList.ToDictionary(edge => edge.Id, StringComparer.Ordinal));
        Fluid = fluid;
    }

    /// <summary>
    /// Unique network identifier.
    /// </summary>
    public string Id { get; }

    public IReadOnlyDictionary<string, Node> Nodes { get; }

    public IReadOnlyDictionary<string, Edge> Edges { get; }

    public Fluid? Fluid { get; }

    private static void ValidateUniqueModelIds(IReadOnlyCollection<Node> nodes, IReadOnlyCollection<Edge> edges)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);

        foreach (var node in nodes)
        {
            if (!ids.Add(node.Id))
            {
                throw new ArgumentException($"Duplicate model id '{node.Id}'.", nameof(nodes));
            }
        }

        foreach (var edge in edges)
        {
            if (!ids.Add(edge.Id))
            {
                throw new ArgumentException($"Duplicate model id '{edge.Id}'.", nameof(edges));
            }
        }
    }

    private static void ValidateEdgeReferences(IReadOnlyCollection<Node> nodes, IReadOnlyCollection<Edge> edges)
    {
        var nodeIds = nodes.Select(node => node.Id).ToHashSet(StringComparer.Ordinal);

        foreach (var edge in edges)
        {
            if (!nodeIds.Contains(edge.FromNodeId))
            {
                throw new ArgumentException($"Edge '{edge.Id}' references unknown from-node '{edge.FromNodeId}'.", nameof(edges));
            }

            if (!nodeIds.Contains(edge.ToNodeId))
            {
                throw new ArgumentException($"Edge '{edge.Id}' references unknown to-node '{edge.ToNodeId}'.", nameof(edges));
            }
        }
    }
}
