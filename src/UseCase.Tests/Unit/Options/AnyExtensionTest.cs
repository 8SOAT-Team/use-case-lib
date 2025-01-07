using CleanArch.UseCase.Options;
using FluentAssertions;
using UseCase.Tests.MockStudio;

namespace UseCase.Tests.Unit.Options;

public sealed class AnyExtensionTest
{
    private record MockModel();

    [Fact]
    public void ToAny_Given_HasValue_Should_ReturnSomeWithValue()
    {
        // Arrange
        var input = FakeIt.Faker.Random.Word();

        // Act
        var sut = input.ToAny();

        // Assert
        sut.Should().BeOfType<Some<string>>(because: "Quando existe um valor retornamos Some");
        sut.HasValue.Should().BeTrue(because: "Deve existir um valor não nulo no retorno");
        sut.Value.Should().Be(input, because: "é o valor original");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ToAny_Given_HasEmptyOrNullString_Should_BeEmpty(string? input)
    {
        // Arrange
        // Act
        var sut = input.ToAny();

        // Assert
        sut.Should().BeOfType<Empty<string>>(because: "Não existe valor");
        sut.HasValue.Should().BeFalse(because: "Valor está vazio");
    }

    [Fact]
    public void ToAny_Given_NullableObject_Should_BeEmpty()
    {

        // Arrange
        MockModel input = null!;

        // Act
        var sut = input.ToAny();

        // Assert
        sut.Should().BeOfType<Empty<MockModel>>(because: "Não existe valor");
        sut.HasValue.Should().BeFalse(because: "Valor está vazio");
    }
}