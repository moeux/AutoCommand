using AutoCommand.Utils;
using FluentAssertions;

namespace AutoCommand.Tests.Utils;

public class EnvironmentUtilsTest
{
    [Fact]
    public void GetVariable_Should_ReturnVariable()
    {
        // Arrange
        var name = "TEST";
        var value = "name";
        Environment.SetEnvironmentVariable(name, value);

        // Act
        var result = EnvironmentUtils.GetVariable(name);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void GetVariable_Should_ReturnFallback()
    {
        // Act
        var fallback = "fallback";
        var result = EnvironmentUtils.GetVariable("SOME_UNKNOWN_VARIABLE", fallback);

        // Assert
        result.Should().Be(fallback);
    }
}