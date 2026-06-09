namespace FDS.Core.Models;

/// <summary>
/// Basic fluid definition. Density is stored in kg/m^3. Reference temperature
/// uses explicit deg C/K separation through Temperature.
/// </summary>
public sealed class Fluid
{
    public Fluid(
        string id,
        string name,
        double densityKilogramsPerCubicMeter,
        Temperature? referenceTemperature = null)
    {
        Id = Validation.RequiredId(id, nameof(id));
        Name = Validation.RequiredName(name, nameof(name));
        DensityKilogramsPerCubicMeter = Validation.EnsureNonNegativeFinite(
            densityKilogramsPerCubicMeter,
            nameof(densityKilogramsPerCubicMeter));
        ReferenceTemperature = referenceTemperature;
    }

    /// <summary>
    /// Unique fluid identifier.
    /// </summary>
    public string Id { get; }

    public string Name { get; }

    /// <summary>
    /// Density in kg/m^3.
    /// </summary>
    public double DensityKilogramsPerCubicMeter { get; }

    public Temperature? ReferenceTemperature { get; }
}
