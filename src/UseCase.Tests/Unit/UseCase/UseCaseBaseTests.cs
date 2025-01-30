using AutoBogus;
using CleanArch.UseCase.Faults;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using UseCase.Tests.MockStudio.Mocks;

namespace UseCase.Tests.Unit.UseCase;

public class UseCaseBaseTests
{
    
    private readonly MockUseCase _sut = new(Substitute.For<ILogger<MockUseCase>>());

    [Fact]
    public async Task ResolveAsync_Should_ReturnValidResult_WhenExecutionHasSucceed()
    {
        // Arrange
        var command = AutoFaker.Generate<MockCommand>();

        // Act
        var result = await _sut.ResolveAsync(command);

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new MockResult($"Processed {command.Input}"));
    }

    [Fact]
    public async Task ResolveAsync_Should_ReturnEmptyWhenExecutionFails()
    {
        // Arrange
        var command = new MockCommand("fail");

        // Act
        var result = await _sut.ResolveAsync(command);

        // Assert
        result.HasValue.Should().BeFalse();
        _sut.GetErrors().Should().ContainSingle(e => e.Code == UseCaseErrorType.InternalError);
    }

    [Fact]
    public async Task ResolveAsync_Should_ReturnEmptyAndRegisterError_WhenItHasThrownAnException()
    {
        // Arrange
        var command = new MockCommand("ucex");

        // Act
        var result = await _sut.ResolveAsync(command);

        // Assert
        result.HasValue.Should().BeFalse();
        _sut.GetErrors().Should().ContainSingle(e => e.Code == UseCaseErrorType.BadRequest);
    }

    [Fact]
    public async Task ResolveAsync_Should_ThrowException_WhenThrowExceptionOnFailureIsTrue()
    {
        // Arrange
        var logger = Substitute.For<ILogger<MockUseCaseThrowException>>();
        var command = new MockCommand("fail");
        var sutWithException = new MockUseCaseThrowException(logger);

        // Act
        Func<Task> act = async () => await sutWithException.ResolveAsync(command);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Execution failed");
        _sut.GetErrors().Should().BeEmpty();
    }

    [Fact]
    public void GetErrors_Should_ReturnAddedErrors()
    {
        // Arrange
        var errors = AutoFaker.Generate<UseCaseError>(3);
        foreach (var error in errors)
        {
            _sut.MockAddError(error);
        }

        // Act
        var result = _sut.GetErrors();

        // Assert
        result.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void AddError_ShouldAddMultipleErrors()
    {
        // Arrange
        var errors = AutoFaker.Generate<UseCaseError>(3);

        // Act
        _sut.MockAddError(errors);

        // Assert
        _sut.GetErrors().Should().BeEquivalentTo(errors);
    }

    [Fact]
    public async Task IsFailure_Should_BeFalse_WhenHasNoError()
    {
        // Arrange
        var command = AutoFaker.Generate<MockCommand>();

        // Act
        _ = await _sut.ResolveAsync(command);

        // Assert
        _sut.IsFailure.Should().BeFalse();
    }


    [Fact]
    public async Task IsFailure_Should_BeTrue_WhenRegisteredAnyError()
    {
        // Arrange
        var command = new MockCommand("fail");

        // Act
        var result = await _sut.ResolveAsync(command);

        // Assert
        _sut.IsFailure.Should().BeTrue();
    }
}