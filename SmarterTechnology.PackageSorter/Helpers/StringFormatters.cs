namespace SmarterTechnology.PackageSorter.Helpers;

public static class StringFormatters
{
    public static string FormatToUpperCase(this Enum value) =>
        value.ToString().ToUpperInvariant();
}
