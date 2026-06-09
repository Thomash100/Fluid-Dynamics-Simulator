using FDS.Hydraulics.Internal;

namespace FDS.Hydraulics.Models;

/// <summary>
/// Basic Kv/Kvs data for a valve. Kv and Kvs are stored in m^3/h using the
/// common metric convention for water at 1 bar pressure drop.
/// </summary>
public sealed class ValveFlowCoefficient
{
    public ValveFlowCoefficient(double kvCubicMetersPerHour, double kvsCubicMetersPerHour)
    {
        KvCubicMetersPerHour = HydraulicValidation.EnsurePositiveFinite(
            kvCubicMetersPerHour,
            nameof(kvCubicMetersPerHour));
        KvsCubicMetersPerHour = HydraulicValidation.EnsurePositiveFinite(
            kvsCubicMetersPerHour,
            nameof(kvsCubicMetersPerHour));

        if (KvCubicMetersPerHour > KvsCubicMetersPerHour)
        {
            throw new ArgumentOutOfRangeException(
                nameof(kvCubicMetersPerHour),
                kvCubicMetersPerHour,
                "Kv cannot be greater than Kvs.");
        }
    }

    /// <summary>
    /// Current valve flow coefficient in m^3/h.
    /// </summary>
    public double KvCubicMetersPerHour { get; }

    /// <summary>
    /// Fully open valve flow coefficient in m^3/h.
    /// </summary>
    public double KvsCubicMetersPerHour { get; }

    public double OpeningRatio => KvCubicMetersPerHour / KvsCubicMetersPerHour;
}
