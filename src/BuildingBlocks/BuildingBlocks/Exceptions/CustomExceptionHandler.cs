
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BuildingBlocks.Exceptions
{
    public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger) : IExceptionHandler
    {   
        // Mora ova metoda i to async zbog interface
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            // ValueTask -> moze metoda biti i async i sync
            // ValueTask pokriva i Task(async) i sync slucaj.

            logger.LogError($"Error message {exception.Message}");

            /* Pattern matching with switch.
              Ako exxception je neki od napisanih u ovom folderu, ValidationException (iz FluentValidaiton) then
             evaluate exception object and assing values to a tuple (Detail, Title, StatusCode) jer ova tri polja
             imaju u Global Excpetion Handler u Program.cs */
            (string Detail, string Title, int StatusCode) details = exception switch
            {
                InternalServerException =>
                (
                    exception.Message, // Detail
                    exception.GetType().Name, // Title
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError // StatusCode
                ),

                BadRequestException =>
                (
                    exception.Message,
                    exception.GetType().Name,
                    context.Response.StatusCode = StatusCodes.Status400BadRequest
                ),

                // Ovo pokriva i ProductNotFoundException jer ta klasa nasledila NotFoundException
                NotFoundException =>
                (
                    exception.Message,
                    exception.GetType().Name,
                    context.Response.StatusCode = StatusCodes.Status404NotFound
                ),

                /* Ovo znamo da je iz FluentValidation. Pazi, ValidationException moze using System.ComponentModel.DataAnnotations
                ali to nam ne treba, vec to obrisemo i upisemo using FluentValidation. */
                ValidationException =>
                (
                    exception.Message,
                    exception.GetType().Name,
                    context.Response.StatusCode = StatusCodes.Status400BadRequest
                ),

                // Default
                _ => 
                (
                    exception.Message,
                    exception.GetType().Name,
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError
                )
            };

            var problemDetails = new ProblemDetails
            {
                Title = details.Title,
                Detail = details.Detail,
                Status = details.StatusCode,
                Instance = context.Request.Path
            };

            // Extension method - dodajem traceId custom metodu na ProblemDetails klasu, bez da modifikujem tu klasu
            // Prvi argument u Add je ime te metode, a ostali argumenti su  argumenti te metode
            problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

            if (exception is ValidationException validationException)
                // Dodajem novu  extension metodu ValidationErrors na ProblemDetails klasu
                problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);

            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

            return true;
        }
    }
}
