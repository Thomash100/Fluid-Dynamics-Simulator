namespace FDS.Core.Models;

internal static class Validation
{
    public static string RequiredId(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Identifier cannot be empty.", parameterName);
        }

        return value.Trim();
    }

    public static string RequiredName(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Name cannot be empty.", parameterName);
        }

        return value.Trim();
    }

    public static double EnsureFinite(double value, string parameterName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be finite.");
        }

        return value;
    }

    public static double? EnsureFiniteOrNull(double? value, string parameterName)
    {
        return value.HasValue ? EnsureFinite(value.Value, parameterName) : null;
    }

    public static double EnsureNonNegativeFinite(double value, string parameterName)
    {
        EnsureFinite(value, parameterName);

        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");
        }

        return value;
    }

    public static double EnsurePositiveFinite(double value, string parameterName)
    {
        EnsureFinite(value, parameterName);

        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be greater than zero.");
        }

        return value;
    }
}
