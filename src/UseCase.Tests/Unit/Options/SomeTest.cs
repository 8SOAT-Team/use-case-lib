using CleanArch.UseCase.Options;
using FluentAssertions;
using UseCase.Tests.MockStudio;

namespace UseCase.Tests.Unit.Options;

public sealed class SomeTest
{
    [Fact]
    public void Given_Some_Should_HasNestedValueAndHasValueTrue()
    {
        // Arrange
        var input = FakeIt.Faker.Random.Word();

        // Act
        var sut = new Some<string>(input);

        // Assert
        sut.Value.Should().Be(input);
        sut.HasValue.Should().BeTrue();
    }

    [Fact]
    public void Given_Some_NullValue_Should_ThrowException()
    {
        // Arrange
        string input = null!;


        // Act
        var act = () => new Some<string>(input);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}