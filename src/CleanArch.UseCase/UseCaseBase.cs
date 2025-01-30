using CleanArch.UseCase.Faults;
using CleanArch.UseCase.Options;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace CleanArch.UseCase;

public abstract class UseCaseBase<TLogContext, TCommand, TOut>(ILogger<TLogContext> logger)
    : IUseCase<TCommand, TOut> where TOut : class
{
    protected readonly ILogger Logger = logger;
    private readonly List<UseCaseError> _useCaseError = [];
    protected virtual bool ThrowExceptionOnFailure => false;

    public bool IsFailure => _useCaseError.Count != 0;

    protected void AddError(UseCaseError error) => _useCaseError.Add(error);
    protected void AddError(IEnumerable<UseCaseError> errors) => _useCaseError.AddRange(errors);

    public virtual async Task<Any<TOut>> ResolveAsync(TCommand command)
    {
        Logger.LogDebug("Comando recebido: {comando}", JsonSerializer.Serialize(command));

        try
        {
            Logger.LogDebug("Iniciando execucao do comando");

            var result = await Execute(command);

            Logger.LogDebug("Resultado {resultado}",
                result is null ? null : JsonSerializer.Serialize(result));

            return result.ToAny();
        }
        catch (UseCaseException ucex)
        {
            AddError(new UseCaseError(ucex.Code, ucex.Message));
            Logger.LogError("Erro: {exceptionMessage} innerException: {innerException}", ucex.Message,
                ucex.InnerException);
        }
        catch (Exception ex)
        {
            AddError(new UseCaseError(UseCaseErrorType.InternalError, ex.Message));
            Logger.LogError("Erro: {exceptionMessage} innerException: {innerException}", ex.Message, ex.InnerException);
            
            if (ThrowExceptionOnFailure)
            {
                throw;
            }
        }

        return Any<TOut>.Empty;
    }

    public IReadOnlyCollection<UseCaseError> GetErrors() => _useCaseError;

    protected abstract Task<TOut?>  Execute(TCommand command);
}