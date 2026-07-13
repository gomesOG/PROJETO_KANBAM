using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace FlowBoard.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Executando request: {RequestName}", requestName);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next();
            sw.Stop();

            if (sw.ElapsedMilliseconds > 500)
                logger.LogWarning("Request lenta: {RequestName} levou {Elapsed}ms", requestName, sw.ElapsedMilliseconds);
            else
                logger.LogInformation("Request concluída: {RequestName} em {Elapsed}ms", requestName, sw.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "Erro na request: {RequestName}", requestName);
            throw;
        }
    }
}
