namespace FDS.Core.Models;

/// <summary>
/// Represents a temperature with explicit Celsius and Kelvin accessors.
/// Celsius is intended for user-facing engineering values; Kelvin is the
/// absolute thermodynamic value for internal model state.
/// </summary>
public readonly record struct Temperature
{
    private const double CelsiusToKelvinOffset = 273.15;

    private Temperature(double kelvin)
    {
        Kelvin = kelvin;
    }

    /// <summary>
    /// Absolute temperature in K.
    /// </summary>
    public double Kelvin { get; }

    /// <summary>
    /// Engineering/display temperature in deg C.
    /// </summary>
    public double Celsius => Kelvin - CelsiusToKelvinOffset;

    public static Temperature FromCelsius(double celsius)
    {
        Validation.EnsureFinite(celsius, nameof(celsius));

        var kelvin = celsius + CelsiusToKelvinOffset;
        if (kelvin < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(celsius), celsius, "Temperature cannot be below absolute zero.");
        }

        return new Temperature(kelvin);
    }

    public static Temperature FromKelvin(double kelvin)
    {
        Validation.EnsureFinite(kelvin, nameof(kelvin));

        if (kelvin < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(kelvin), kelvin, "Temperature in K cannot be negative.");
        }

        return new Temperature(kelvin);
    }
}
