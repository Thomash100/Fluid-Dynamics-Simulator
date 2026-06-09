using FDS.Core.Models;

namespace FDS.Core.Tests;

public sealed class TemperatureTests
{
    [Fact]
    public void FromCelsius_StoresKelvinSeparately()
    {
        var temperature = Temperature.FromCelsius(20);

        Assert.Equal(20, temperature.Celsius, precision: 6);
        Assert.Equal(293.15, temperature.Kelvin, precision: 6);
    }

    [Fact]
    public void FromKelvin_ExposesCelsius()
    {
        var temperature = Temperature.FromKelvin(293.15);

        Assert.Equal(293.15, temperature.Kelvin, precision: 6);
        Assert.Equal(20, temperature.Celsius, precision: 6);
    }

    [Fact]
    public void FromKelvin_RejectsNegativeKelvin()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Temperature.FromKelvin(-1));
    }

    [Fact]
    public void FromCelsius_RejectsBelowAbsoluteZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Temperature.FromCelsius(-273.16));
    }
}
