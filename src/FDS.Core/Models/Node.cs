namespace FDS.Core.Models;

/// <summary>
/// A network node. Pressure is stored in Pa; temperature uses the Temperature
/// value object to keep deg C and K explicit.
/// </summary>
public sealed class Node
{
    public Node(string id, string? name = null, double? pressurePascals = null, Temperature? temperature = null)
    {
        Id = Validation.RequiredId(id, nameof(id));
        Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        PressurePascals = Validation.EnsureFiniteOrNull(pressurePascals, nameof(pressurePascals));
        Temperature = temperature;
    }

    /// <summary>
    /// Unique node identifier within a network.
    /// </summary>
    public string Id { get; }

    public string? Name { get; }

    /// <summary>
    /// Pressure in Pa.
    /// </summary>
    public double? PressurePascals { get; }

    public Temperature? Temperature { get; }
}
