using CleanArch.UseCase;
using CleanArch.UseCase.Faults;
using Microsoft.Extensions.Logging;

namespace UseCase.Tests.MockStudio.Mocks;

public class MockUseCase(ILogger<MockUseCase> logger) : UseCaseBase<MockUseCase, MockCommand, MockResult>(logger)
{
    protected override Task<MockResult> Execute(MockCommand command)
    {
        return command.Input switch
        {
            "fail" => throw new Exception("Execution failed"),
            "ucex" => throw new UseCaseException(UseCaseErrorType.BadRequest, "Business error occurred"),
            _ => Task.FromResult(new MockResult($"Processed {command.Input}"))
        };
    }

    public void MockAddError(UseCaseError error) => AddError(error);
    public void MockAddError(IEnumerable<UseCaseError> errors) => AddError(errors);
}