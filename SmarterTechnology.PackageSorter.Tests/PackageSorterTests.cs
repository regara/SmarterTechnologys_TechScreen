using FluentAssertions;
using SmarterTechnology.PackageSorter.Helpers;

namespace SmarterTechnology.PackageSorter.Tests;

[TestClass]
public class PackageSorterTests
{
    [TestMethod()]
    public void Sort_LowercasePromptStyleMethod_ReturnsExpectedResult()
    {
        var result = PackageSorter.sort(10d, 10d, 10d, 5d);

        result.Should().Be(StackType.Standard.FormatToUpperCase());
    }

    [TestMethod()]
    public void Sort_ReturnsStandard_WhenPackageIsNeitherBulkyNorHeavy()
    {
        var result = PackageSorter.Sort(10d, 10d, 10d, 5d);

        result.Should().Be(StackType.Standard.FormatToUpperCase());
    }

    [TestMethod()]
    public void Sort_ReturnsSpecial_WhenPackageIsHeavyOnly()
    {
        var result = PackageSorter.Sort(10d, 10d, 10d, 20d);

        result.Should().Be(StackType.Special.FormatToUpperCase());
    }

    [TestMethod()]
    public void Sort_ReturnsSpecial_WhenPackageIsBulkyOnlyByDimension()
    {
        var result = PackageSorter.Sort(150d, 10d, 10d, 19.99d);

        result.Should().Be(StackType.Special.FormatToUpperCase());
    }

    [TestMethod()]
    public void Sort_ReturnsSpecial_WhenPackageIsBulkyOnlyByVolume()
    {
        var result = PackageSorter.Sort(100d, 100d, 100d, 19.99d);

        result.Should().Be(StackType.Special.FormatToUpperCase());
    }

    [TestMethod()]
    public void Sort_ReturnsRejected_WhenPackageIsBothHeavyAndBulky()
    {
        var result = PackageSorter.Sort(100d, 100d, 100d, 20d);

        result.Should().Be(StackType.Rejected.FormatToUpperCase());
    }

    [TestMethod()]
    public void Sort_HandlesDecimalValues_CorrectlyBelowThresholds()
    {
        var result = PackageSorter.Sort(149.9d, 149.9d, 44d, 19.999d);

        result.Should().Be(StackType.Standard.FormatToUpperCase());
    }

    [TestMethod()]
    [DataRow(0d, 1d, 1d, "width")]
    [DataRow(1d, 0d, 1d, "height")]
    [DataRow(1d, 1d, 0d, "length")]
    [DataRow(-1d, 1d, 1d, "width")]
    public void Sort_ThrowsOutOfRange_WhenDimensionIsNotPositive(double width, double height, double length, string paramName)
    {
        var action = () => PackageSorter.Sort(width, height, length, 1d);

        action.Should()
            .Throw<ArgumentOutOfRangeException>()
            .Which.ParamName.Should().Be(paramName);
    }

    [TestMethod()]
    public void Sort_ThrowsOutOfRange_WhenMassIsNegative()
    {
        var action = () => PackageSorter.Sort(1d, 1d, 1d, -0.01d);

        action.Should()
            .Throw<ArgumentOutOfRangeException>()
            .Which.ParamName.Should().Be("mass");
    }

    [TestMethod]
    [DataRow(double.NaN, 1d, 1d, 1d, "width")]
    [DataRow(1d, double.PositiveInfinity, 1d, 1d, "height")]
    [DataRow(1d, 1d, double.NegativeInfinity, 1d, "length")]
    [DataRow(1d, 1d, 1d, double.NaN, "mass")]
    public void Sort_ThrowsArgumentException_WhenValueIsNotFinite(double width, double height, double length, double mass, string paramName)
    {
        var action = () => PackageSorter.Sort(width, height, length, mass);

        action.Should()
            .Throw<ArgumentException>()
            .Which.ParamName.Should().Be(paramName);
    }
}
