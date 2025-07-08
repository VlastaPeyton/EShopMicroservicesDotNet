
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse> 
        where TResponse : notnull
    {   /* IRequest, jer se logging radi i za Command i Query, a ICommand/IQuery implementiraju IRequest 
         IPipelineBehaviour je iz MediatR. 
        */

        // Mora ovam etoda zbog interface i to async da dodam
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Log incoming Request
            logger.LogInformation($" Handle Request = {typeof(TRequest).Name} - Response={typeof(TResponse).Name} - RequestData={request}");

            var timer = new Stopwatch();
            timer.Start();

            var response = await next(); // Poziva se Handle metod u Command/QueryHandler
            timer.Stop();

            var timeTaken = timer.Elapsed;

            if (timeTaken.Seconds > 3)
                logger.LogWarning($"The request took {timeTaken}");

            logger.LogInformation($" Handled Request={typeof(TRequest).Name} with Response={typeof(TResponse).Name}");
            
            return response;
        }
    }
}
