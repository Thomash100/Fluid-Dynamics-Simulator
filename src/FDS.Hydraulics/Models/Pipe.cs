using FDS.Core.Models;

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
        Id = RequiredId(id, nameof(id));
        FromNodeId = OptionalId(fromNodeId, nameof(fromNodeId));
        ToNodeId = OptionalId(toNodeId, nameof(toNodeId));
        LengthMeters = EnsureNonNegativeFinite(lengthMeters, nameof(lengthMeters));
        InnerDiameterMeters = EnsurePositiveFinite(innerDiameterMeters, nameof(innerDiameterMeters));
        RoughnessMeters = EnsureNonNegativeFinite(roughnessMeters, nameof(roughnessMeters));
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

    private static string RequiredId(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Identifier cannot be empty.", parameterName);
        }

        return value.Trim();
    }

    private static string? OptionalId(string? value, string parameterName)
    {
        if (value is null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Identifier cannot be empty when provided.", parameterName);
        }

        return value.Trim();
    }

    private static double EnsureFinite(double value, string parameterName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        return value;
    }

    private static double EnsureNonNegativeFinite(double value, string parameterName)
    {
        EnsureFinite(value, parameterName);

        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");
        }

        return value;
    }

    private static double EnsurePositiveFinite(double value, string parameterName)
    {
        EnsureFinite(value, parameterName);

        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be greater than zero.");
        }

        return value;
    }
}
