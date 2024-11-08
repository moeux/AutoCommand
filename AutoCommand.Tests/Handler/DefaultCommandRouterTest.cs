using AutoCommand.Handler;
using FluentAssertions;

namespace AutoCommand.Tests.Handler;

public class DefaultCommandRouterTest
{
    [Fact]
    public void TryRegister_Returns_True()
    {
        // Arrange
        var fakeCommandHandler = new FakeCommandHandler();
        var commandRouter = new DefaultCommandRouter();

        // Act
        var result = commandRouter.TryRegister(fakeCommandHandler);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TryRegister_Returns_False()
    {
        // Arrange
        var fakeCommandHandler = new FakeCommandHandler();
        var commandRouter = new DefaultCommandRouter();

        // Act
        commandRouter.TryRegister(fakeCommandHandler);
        var result = commandRouter.TryRegister(fakeCommandHandler);

        // Assert
        result.Should().BeFalse();
    }


    [Fact]
    public void TryRegister_Throws_ArgumentNullException()
    {
        // Arrange
        var commandRouter = new DefaultCommandRouter();

        // Act
        var result = () => commandRouter.TryRegister(null);

        // Assert
        result.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Register_Throws_ArgumentNullException()
    {
        // Arrange
        var commandRouter = new DefaultCommandRouter();

        // Act
        var result = () => commandRouter.Register(null);

        // Assert
        result.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Unregister_Returns_True()
    {
        // Arrange
        var fakeCommandHandler = new FakeCommandHandler();
        var commandRouter = new DefaultCommandRouter([fakeCommandHandler]);

        // Act
        var result = commandRouter.Unregister(fakeCommandHandler);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Unregister_Returns_False_WhenNull()
    {
        // Arrange
        var commandRouter = new DefaultCommandRouter();

        // Act
        var result = commandRouter.Unregister(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Unregister_Returns_False()
    {
        // Arrange
        var fakeCommandHandler = new FakeCommandHandler();
        var commandRouter = new DefaultCommandRouter();

        // Act
        var result = commandRouter.Unregister(fakeCommandHandler);

        // Assert
        result.Should().BeFalse();
    }
}