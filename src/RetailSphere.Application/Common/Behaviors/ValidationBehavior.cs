using FluentValidation;
using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Common.Behaviors;

/// <summary>
/// Runs all registered FluentValidation validators for a request before it
/// reaches its handler. Handlers never re-validate — if a validator fails,
/// the pipeline short-circuits and returns a Result/Result&lt;T&gt; failure
/// (ErrorType.Validation) rather than throwing, when the response type supports it.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var message = string.Join("; ", failures.Select(f => f.ErrorMessage));
        var error = Error.Validation("Validation.Failed", message);

        // TResponse is expected to be Result or Result<T> for every command/query
        // in this codebase; this cast is the one deliberate exception to "handlers
        // don't know about MediatR internals" so validation failures never throw.
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = typeof(Result)
                .GetMethod(nameof(Result.Failure), 1, [typeof(Error)])!
                .MakeGenericMethod(typeof(TResponse).GetGenericArguments()[0]);

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        throw new ValidationException(failures);
    }
}
