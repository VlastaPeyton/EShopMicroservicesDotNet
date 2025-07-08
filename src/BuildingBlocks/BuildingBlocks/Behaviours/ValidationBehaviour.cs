using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> (IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>
        // ICommand jer se validation radi samo za Command
    {  /* - IPipelineBehaviour je iz MediatR. 
          - IValidator<TRequest> isto sto i IValidator<CreateProductCommand> u CreateProductCommandHandler.cs
          - IEnumerable koristim jer ocu sve zeljeno da validiram
          - IValidator znace da se odnosi na CreateProductCommandValidator zbog DI u MediatR Pipeline u Program.cs jer je CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
          
          Ovo je objasnjenje za svaku Command kome smo dodali validation u bilo kom service. */
          
        // Mora metoda zbog interface i to async
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            /* Request je incoming Request from Client to Endpoint.
               Next je next pipeline behavior or actual handle method sa slike "How MediatR pipeline behavior works"
            */

            // ValidationContext comes from FluentValidation 
            var context = new ValidationContext<TRequest>(request);

            // Call ValidateAsync metod for each Handle metod 
            // Task.WhenAll = complete all validations operations
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));    

            // Check for any errror in any validation result = ide kroz CreateProductCommandValidator i ulazi u RuleFor svaki 
            var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();

            // If any error occured, throw ValidationException 
            if (failures.Any())
                throw new ValidationException(failures);

            /* next() will run next pipeline behaviour which will be my actual Handle method (from CreateProductCommandHandler), jer
             * kad smo validiraili, idemo na Handle metodu */
            return await next();

        }
    }
}
