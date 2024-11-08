using AutoCommand.Utils;
using FluentAssertions;

namespace AutoCommand.Tests.Utils;

public class EnumParserTest
{
    [Fact]
    public void ParseToEnumValue_Should_ReturnEnumValue()
    {
        // Arrange 
        var value = "Fake";

        // Act
        var result = EnumParser.ParseToEnumValue<FakeEnum>(value);

        // Assert
        result.Should().Be(FakeEnum.Fake);
    }

    [Theory]
    [InlineData("fake", false)]
    [InlineData("unknown", true)]
    public void ParseToEnumValue_Should_ReturnNull(string value, bool ignoreCase)
    {
        // Act
        var result = EnumParser.ParseToEnumValue<FakeEnum>(value, ignoreCase);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseToEnumValues_Should_ReturnEmptyList()
    {
        // Arrange
        var values = new string[] { };

        // Act
        var result = EnumParser.ParseToEnumValues<FakeEnum>(values);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseToEnumValues_Should_ReturnEmptyList_WhenNull()
    {
        // Act
        var result = EnumParser.ParseToEnumValues<FakeEnum>(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseToEnumValues_Should_ReturnEmptyList_WhenValuesNullOrWhiteSpace()
    {
        // Arrange
        var values = new[] { null, string.Empty, " " };

        // Act
        var result = EnumParser.ParseToEnumValues<FakeEnum>(values);

        // Assert
        result.Should().BeEmpty();
    }
}