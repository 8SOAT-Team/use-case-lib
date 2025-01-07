using System.Text.Json;
using AutoBogus;
using CleanArch.UseCase;
using CleanArch.UseCase.Faults;
using CleanArch.UseCase.Logging;
using FluentAssertions;
using NSubstitute;

namespace UseCase.Tests.Unit.UseCase;

public class UseCaseBaseTests
{
    private record MockCommand(string Input);
    private record MockResult(string Output);

    private class MockUseCase(ILogger logger) : UseCaseBase<MockCommand, MockResult>(logger)
    {
        protected override Task<MockResult?> Execute(MockCommand command)
        {
            if (command.Input == "fail")
                throw new Exception("Execution failed");

            if (command.Input == "ucex")
                throw new UseCaseException(UseCaseErrorType.BadRequest, "Business error occurred");

            return Task.FromResult(new MockResult($"Processed {command.Input}"))!;
        }

        public void MockAddError(UseCaseError error) => AddError(error);
        public void MockAddError(IEnumerable<UseCaseError> errors) => AddError(errors);
    }

    private class MockUseCaseThrowException(ILogger logger) : MockUseCase(logger)
    {
        protected override bool ThrowExceptionOnFailure => true;
    }

    private readonly ILogger _mockLogger;
    private readonly MockUseCase _sut;

    public UseCaseBaseTests()
    {
        _mockLogger = Substitute.For<ILogger>();
        _sut = new MockUseCase(_mockLogger);
    }

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
        _mockLogger.Received().LogInfo(Arg.Any<string>());
        _mockLogger.Received().LogDebug(Arg.Any<string>());
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
        _mockLogger.Received().LogError(Arg.Any<string>(), Arg.Any<Exception>());
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
        _mockLogger.Received().LogError(Arg.Any<string>(), null);
    }

    [Fact]
    public async Task ResolveAsync_Should_ThrowException_WhenThrowExceptionOnFailureIsTrue()
    {
        // Arrange
        var command = new MockCommand("fail");
        var sutWithException = new MockUseCaseThrowException(_mockLogger);

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
    public async Task ResolveAsync_Should_RegisterStartAndEndLogs()
    {
        // Arrange
        var command = AutoFaker.Generate<MockCommand>();

        // Act
        await _sut.ResolveAsync(command);

        // Assert
        _mockLogger.Received().LogInfo(Arg.Is<string>(msg => msg.Contains("Iniciando Resolve")));
        _mockLogger.Received().LogInfo(Arg.Is<string>(msg => msg.Contains("Execucao concluida")));
    }

    [Fact]
    public async Task ResolveAsync_Should_SerializeCommandCorrectly()
    {
        // Arrange
        var command = new MockCommand("test-input");
        var expectedSerialization = JsonSerializer.Serialize(command);

        // Act
        await _sut.ResolveAsync(command);

        // Assert
        _mockLogger.Received().LogDebug(Arg.Is<string>(msg => msg.Contains(expectedSerialization)));
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
