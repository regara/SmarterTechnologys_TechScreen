using FluentAssertions;

namespace SmarterTechnology.PackageSorter.Tests;

[TestClass]
public class PackageSorterTests
{
    [TestMethod]
    public void Sort_ReturnsStandard_WhenNeitherBulkyNorHeavy()
    {
        var result = PackageSorter.Sort(10d, 10d, 10d, 5d);

        result.Should().Be(StackType.Standard);
    }

    [TestMethod]
    [DataRow(10d, 10d, 10d, 5d, StackType.Standard, DisplayName = "STANDARD when neither bulky nor heavy")]
    [DataRow(10d, 10d, 10d, 20d, StackType.Special, DisplayName = "SPECIAL when mass is exactly threshold")]
    [DataRow(1d, 1d, 1d, 19.999d, StackType.Standard, DisplayName = "STANDARD when mass is just below threshold")]
    [DataRow(150d, 1d, 1d, 19.99d, StackType.Special, DisplayName = "SPECIAL when width is exactly threshold")]
    [DataRow(149.999d, 1d, 1d, 19.99d, StackType.Standard, DisplayName = "STANDARD when width is just below threshold")]
    [DataRow(1d, 150d, 1d, 19.99d, StackType.Special, DisplayName = "SPECIAL when height is exactly threshold")]
    [DataRow(1d, 1d, 150d, 19.99d, StackType.Special, DisplayName = "SPECIAL when length is exactly threshold")]
    [DataRow(100d, 100d, 100d, 19.99d, StackType.Special, DisplayName = "SPECIAL when volume is exactly threshold")]
    [DataRow(100d, 100d, 99.999d, 19.99d, StackType.Standard, DisplayName = "STANDARD when volume is just below threshold")]
    [DataRow(100d, 100d, 100d, 20d, StackType.Rejected, DisplayName = "REJECTED when both bulky and heavy")]
    [DataRow(149.9d, 149.9d, 44d, 19.999d, StackType.Standard, DisplayName = "STANDARD for decimal values below all thresholds")]
    public void Sort_ReturnsExpectedStack_ForClassificationScenarios(
        double width,
        double height,
        double length,
        double mass,
        StackType expectedStack)
    {
        var result = PackageSorter.Sort(width, height, length, mass);

        result.Should().Be(expectedStack);
    }

    [TestMethod]
    [DataRow(0d, 1d, 1d, "width", DisplayName = "Throws when width is zero")]
    [DataRow(1d, 0d, 1d, "height", DisplayName = "Throws when height is zero")]
    [DataRow(1d, 1d, 0d, "length", DisplayName = "Throws when length is zero")]
    [DataRow(-1d, 1d, 1d, "width", DisplayName = "Throws when width is negative")]
    [DataRow(1d, -1d, 1d, "height", DisplayName = "Throws when height is negative")]
    [DataRow(1d, 1d, -1d, "length", DisplayName = "Throws when length is negative")]
    public void Sort_ThrowsOutOfRange_WhenDimensionIsNotPositive(
        double width,
        double height,
        double length,
        string paramName)
    {
        var action = () => PackageSorter.Sort(width, height, length, 1d);

        action.Should()
            .Throw<ArgumentOutOfRangeException>()
            .Which.ParamName.Should().Be(paramName);
    }

    [TestMethod]
    public void Sort_ThrowsOutOfRange_WhenMassIsNegative()
    {
        var action = () => PackageSorter.Sort(1d, 1d, 1d, -0.01d);

        action.Should()
            .Throw<ArgumentOutOfRangeException>()
            .Which.ParamName.Should().Be("mass");
    }

    [TestMethod]
    [DataRow(double.NaN, 1d, 1d, 1d, "width", DisplayName = "Throws when width is NaN")]
    [DataRow(1d, double.PositiveInfinity, 1d, 1d, "height", DisplayName = "Throws when height is positive infinity")]
    [DataRow(1d, 1d, double.NegativeInfinity, 1d, "length", DisplayName = "Throws when length is negative infinity")]
    [DataRow(1d, 1d, 1d, double.NaN, "mass", DisplayName = "Throws when mass is NaN")]
    public void Sort_ThrowsArgumentException_WhenValueIsNotFinite(
        double width,
        double height,
        double length,
        double mass,
        string paramName)
    {
        var action = () => PackageSorter.Sort(width, height, length, mass);

        action.Should()
            .Throw<ArgumentException>()
            .Which.ParamName.Should().Be(paramName);
    }
}