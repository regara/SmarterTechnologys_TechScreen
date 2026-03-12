using SmarterTechnology.PackageSorter.Helpers;

namespace SmarterTechnology.PackageSorter;

public static class PackageSorter
{
    public const double VolumeThreshold = 1_000_000d;
    public const double DimensionThreshold = 150d;
    public const double MassThreshold = 20d;

    public static string sort(double width, double height, double length, double mass)
    {
        return Sort(width, height, length, mass);
    }

    public static string Sort(double width, double height, double length, double mass)
    {
        return SortToStack(width, height, length, mass).FormatToUpperCase();
    }

    public static StackType SortToStack(double width, double height, double length, double mass)
    {
        ValidateDimension(width, nameof(width));
        ValidateDimension(height, nameof(height));
        ValidateDimension(length, nameof(length));
        ValidateMass(mass, nameof(mass));

        var volume = width * height * length;
        var isBulky = double.IsInfinity(volume) ||
                      volume >= VolumeThreshold ||
                      width >= DimensionThreshold ||
                      height >= DimensionThreshold ||
                      length >= DimensionThreshold;

        var isHeavy = mass >= MassThreshold;

        if (isBulky && isHeavy)
        {
            return StackType.Rejected;
        }

        if (isBulky || isHeavy)
        {
            return StackType.Special;
        }

        return StackType.Standard;
    }

    private static void ValidateDimension(double value, string paramName)
    {
        ValidateFiniteNumber(value, paramName);
        if (value <= 0d)
        {
            throw new ArgumentOutOfRangeException(paramName, "Dimensions must be greater than 0.");
        }
    }

    private static void ValidateMass(double value, string paramName)
    {
        ValidateFiniteNumber(value, paramName);
        if (value < 0d)
        {
            throw new ArgumentOutOfRangeException(paramName, "Mass must be greater than or equal to 0.");
        }
    }

    private static void ValidateFiniteNumber(double value, string paramName)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentException("Value must be a finite number.", paramName);
        }
    }
}
