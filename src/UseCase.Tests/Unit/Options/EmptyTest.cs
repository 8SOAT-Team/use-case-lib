using CleanArch.UseCase.Options;
using FluentAssertions;

namespace UseCase.Tests.Unit.Options;

public sealed class EmptyTest
{
    [Fact]
    public void Given_Empty_Should_BeValueDefaultAndHasValueFalse()
    {
        // Arrange
        var expectedValue = default(string);

        // Act
        var sut = Empty<string>.Empty;

        // Assert
        sut.Value.Should().Be(expectedValue);
        sut.HasValue.Should().BeFalse();
    }
}
