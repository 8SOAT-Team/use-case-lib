using CleanArch.UseCase.Options;
using FluentAssertions;
using UseCase.Tests.MockStudio;

namespace UseCase.Tests.Unit.Options;

public sealed class AnyTest
{
    [Fact]
    public void Some_Given_InputValue_Should_BeSomeWithInputValueValue()
    {
        // Arrange
        var input = FakeIt.Faker.Random.Word();

        // Act
        var sut = Any<string>.Some(input);

        // Assert
        sut.Should().BeOfType<Some<string>>();
        sut.Value.Should().Be(input);
        sut.HasValue.Should().BeTrue();
    }

    [Fact]
    public void Empty_Given_InputValueIsEmpty_Should_BeEmptyAndHasValueFalse()
    {
        // Arrange
        // Act
        var sut = Any<string>.Empty;

        // Assert
        sut.Should().BeOfType<Empty<string>>();
        sut.HasValue.Should().BeFalse();
    }
}
