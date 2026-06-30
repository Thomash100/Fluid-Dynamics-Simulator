using System.Globalization;

namespace FDS.WindowsApp;

internal enum PressureDisplayUnit
{
    Pascal,
    Kilopascal,
    Bar,
}

internal enum LengthDisplayUnit
{
    Meter,
    Millimeter,
}

internal enum VolumeFlowDisplayUnit
{
    CubicMetersPerSecond,
    LitersPerSecond,
    CubicMetersPerHour,
}

internal sealed class UnitDisplaySettings
{
    public static UnitDisplaySettings Default => new();

    public PressureDisplayUnit PressureUnit { get; set; } = PressureDisplayUnit.Pascal;

    public LengthDisplayUnit LengthUnit { get; set; } = LengthDisplayUnit.Meter;

    public VolumeFlowDisplayUnit VolumeFlowUnit { get; set; } = VolumeFlowDisplayUnit.CubicMetersPerSecond;

    public string PressureUnitText => PressureUnit switch
    {
        PressureDisplayUnit.Pascal => "Pa",
        PressureDisplayUnit.Kilopascal => "kPa",
        PressureDisplayUnit.Bar => "bar",
        _ => PressureUnit.ToString(),
    };

    public string LengthUnitText => LengthUnit switch
    {
        LengthDisplayUnit.Meter => "m",
        LengthDisplayUnit.Millimeter => "mm",
        _ => LengthUnit.ToString(),
    };

    public string VolumeFlowUnitText => VolumeFlowUnit switch
    {
        VolumeFlowDisplayUnit.CubicMetersPerSecond => "m³/s",
        VolumeFlowDisplayUnit.LitersPerSecond => "l/s",
        VolumeFlowDisplayUnit.CubicMetersPerHour => "m³/h",
        _ => VolumeFlowUnit.ToString(),
    };

    public int PressureDecimalPlaces => PressureUnit switch
    {
        PressureDisplayUnit.Pascal => 2,
        PressureDisplayUnit.Kilopascal => 4,
        PressureDisplayUnit.Bar => 6,
        _ => 2,
    };

    public int LengthDecimalPlaces => LengthUnit switch
    {
        LengthDisplayUnit.Meter => 4,
        LengthDisplayUnit.Millimeter => 1,
        _ => 4,
    };

    public int VolumeFlowDecimalPlaces => VolumeFlowUnit switch
    {
        VolumeFlowDisplayUnit.CubicMetersPerSecond => 8,
        VolumeFlowDisplayUnit.LitersPerSecond => 4,
        VolumeFlowDisplayUnit.CubicMetersPerHour => 4,
        _ => 8,
    };

    public decimal PressureIncrement => PressureUnit switch
    {
        PressureDisplayUnit.Pascal => 100M,
        PressureDisplayUnit.Kilopascal => 0.1M,
        PressureDisplayUnit.Bar => 0.001M,
        _ => 100M,
    };

    public decimal LengthIncrement => LengthUnit switch
    {
        LengthDisplayUnit.Meter => 0.01M,
        LengthDisplayUnit.Millimeter => 1M,
        _ => 0.01M,
    };

    public decimal VolumeFlowIncrement => VolumeFlowUnit switch
    {
        VolumeFlowDisplayUnit.CubicMetersPerSecond => 0.001M,
        VolumeFlowDisplayUnit.LitersPerSecond => 0.1M,
        VolumeFlowDisplayUnit.CubicMetersPerHour => 0.1M,
        _ => 0.001M,
    };

    public double ToDisplayPressure(double pressurePascals)
    {
        return PressureUnit switch
        {
            PressureDisplayUnit.Pascal => pressurePascals,
            PressureDisplayUnit.Kilopascal => pressurePascals / 1_000.0,
            PressureDisplayUnit.Bar => pressurePascals / 100_000.0,
            _ => pressurePascals,
        };
    }

    public double ToPascals(double displayPressure)
    {
        return PressureUnit switch
        {
            PressureDisplayUnit.Pascal => displayPressure,
            PressureDisplayUnit.Kilopascal => displayPressure * 1_000.0,
            PressureDisplayUnit.Bar => displayPressure * 100_000.0,
            _ => displayPressure,
        };
    }

    public double ToDisplayLength(double lengthMeters)
    {
        return LengthUnit switch
        {
            LengthDisplayUnit.Meter => lengthMeters,
            LengthDisplayUnit.Millimeter => lengthMeters * 1_000.0,
            _ => lengthMeters,
        };
    }

    public double ToMeters(double displayLength)
    {
        return LengthUnit switch
        {
            LengthDisplayUnit.Meter => displayLength,
            LengthDisplayUnit.Millimeter => displayLength / 1_000.0,
            _ => displayLength,
        };
    }

    public double ToDisplayVolumeFlow(double volumeFlowCubicMetersPerSecond)
    {
        return VolumeFlowUnit switch
        {
            VolumeFlowDisplayUnit.CubicMetersPerSecond => volumeFlowCubicMetersPerSecond,
            VolumeFlowDisplayUnit.LitersPerSecond => volumeFlowCubicMetersPerSecond * 1_000.0,
            VolumeFlowDisplayUnit.CubicMetersPerHour => volumeFlowCubicMetersPerSecond * 3_600.0,
            _ => volumeFlowCubicMetersPerSecond,
        };
    }

    public double ToCubicMetersPerSecond(double displayVolumeFlow)
    {
        return VolumeFlowUnit switch
        {
            VolumeFlowDisplayUnit.CubicMetersPerSecond => displayVolumeFlow,
            VolumeFlowDisplayUnit.LitersPerSecond => displayVolumeFlow / 1_000.0,
            VolumeFlowDisplayUnit.CubicMetersPerHour => displayVolumeFlow / 3_600.0,
            _ => displayVolumeFlow,
        };
    }

    public string FormatPressure(double pressurePascals)
    {
        return $"{FormatNumber(ToDisplayPressure(pressurePascals))} {PressureUnitText}";
    }

    public string FormatLength(double lengthMeters)
    {
        return $"{FormatNumber(ToDisplayLength(lengthMeters))} {LengthUnitText}";
    }

    public string FormatVolumeFlow(double volumeFlowCubicMetersPerSecond)
    {
        return $"{FormatNumber(ToDisplayVolumeFlow(volumeFlowCubicMetersPerSecond))} {VolumeFlowUnitText}";
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("G6", CultureInfo.InvariantCulture);
    }
}
