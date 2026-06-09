using FDS.Core.Models;
using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Hydraulic pipe segment. Length, inner diameter, and roughness are stored in m.
/// This model contains no network-solving behavior.
/// </summary>
public sealed class Pipe
{
    public Pipe(
        string id,
        double lengthMeters,
        double innerDiameterMeters,
        string? fromNodeId = null,
        string? toNodeId = null,
        double roughnessMeters = 0)
    {
        Id = HydraulicValidation.RequiredId(id, nameof(id));
        FromNodeId = HydraulicValidation.OptionalId(fromNodeId, nameof(fromNodeId));
        ToNodeId = HydraulicValidation.OptionalId(toNodeId, nameof(toNodeId));
        LengthMeters = HydraulicValidation.EnsureNonNegativeFinite(lengthMeters, nameof(lengthMeters));
        InnerDiameterMeters = HydraulicValidation.EnsurePositiveFinite(innerDiameterMeters, nameof(innerDiameterMeters));
        RoughnessMeters = HydraulicValidation.EnsureNonNegativeFinite(roughnessMeters, nameof(roughnessMeters));
    }

    public string Id { get; }

    public string? FromNodeId { get; }

    public string? ToNodeId { get; }

    /// <summary>
    /// Pipe length in m.
    /// </summary>
    public double LengthMeters { get; }

    /// <summary>
    /// Inner pipe diameter in m.
    /// </summary>
    public double InnerDiameterMeters { get; }

    /// <summary>
    /// Absolute roughness in m. It is stored for future pressure-loss models;
    /// the current simple friction-factor approximation does not use it.
    /// </summary>
    public double RoughnessMeters { get; }

    public static Pipe FromEdge(Edge edge, double roughnessMeters = 0)
    {
        ArgumentNullException.ThrowIfNull(edge);

        return new Pipe(
            edge.Id,
            edge.LengthMeters,
            edge.DiameterMeters,
            edge.FromNodeId,
            edge.ToNodeId,
            roughnessMeters);
    }
}
