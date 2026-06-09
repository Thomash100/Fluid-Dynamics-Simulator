namespace FDS.Core.Models;

/// <summary>
/// A directed network edge between two nodes. Length is stored in m, diameter
/// in m, volumetric flow in m^3/s, and mass flow in kg/s.
/// </summary>
public sealed class Edge
{
    public Edge(
        string id,
        string fromNodeId,
        string toNodeId,
        double lengthMeters,
        double diameterMeters,
        double? volumetricFlowRateCubicMetersPerSecond = null,
        double? massFlowRateKilogramsPerSecond = null)
    {
        Id = Validation.RequiredId(id, nameof(id));
        FromNodeId = Validation.RequiredId(fromNodeId, nameof(fromNodeId));
        ToNodeId = Validation.RequiredId(toNodeId, nameof(toNodeId));
        LengthMeters = Validation.EnsureNonNegativeFinite(lengthMeters, nameof(lengthMeters));
        DiameterMeters = Validation.EnsurePositiveFinite(diameterMeters, nameof(diameterMeters));
        VolumetricFlowRateCubicMetersPerSecond = Validation.EnsureFiniteOrNull(
            volumetricFlowRateCubicMetersPerSecond,
            nameof(volumetricFlowRateCubicMetersPerSecond));
        MassFlowRateKilogramsPerSecond = Validation.EnsureFiniteOrNull(
            massFlowRateKilogramsPerSecond,
            nameof(massFlowRateKilogramsPerSecond));
    }

    /// <summary>
    /// Unique edge identifier within a network.
    /// </summary>
    public string Id { get; }

    public string FromNodeId { get; }

    public string ToNodeId { get; }

    /// <summary>
    /// Pipe or component length in m.
    /// </summary>
    public double LengthMeters { get; }

    /// <summary>
    /// Hydraulic diameter in m.
    /// </summary>
    public double DiameterMeters { get; }

    /// <summary>
    /// Volumetric flow rate in m^3/s. A negative value may represent flow
    /// opposite to the edge direction.
    /// </summary>
    public double? VolumetricFlowRateCubicMetersPerSecond { get; }

    /// <summary>
    /// Mass flow rate in kg/s. A negative value may represent flow opposite to
    /// the edge direction.
    /// </summary>
    public double? MassFlowRateKilogramsPerSecond { get; }
}
