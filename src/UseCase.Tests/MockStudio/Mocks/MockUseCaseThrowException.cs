using Microsoft.Extensions.Logging;

namespace UseCase.Tests.MockStudio.Mocks;

public class MockUseCaseThrowException(ILogger<MockUseCaseThrowException> logger) : MockUseCase(logger)
{
    protected override bool ThrowExceptionOnFailure => true;
}