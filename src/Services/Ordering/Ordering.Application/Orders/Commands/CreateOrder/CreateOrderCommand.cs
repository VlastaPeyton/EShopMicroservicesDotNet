
using BuildingBlocks.CQRS;
using FluentValidation;
using Ordering.Application.DTOs;

namespace Ordering.Application.Orders.Commands.CreateOrder
{   /* Prvo pogledaj CreateOrderEndpoint.cs u API layer i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */
    public record CreateOrderCommand(OrderDTO Order) : ICommand<CreateOrderResult>; 
    public record CreateOrderResult(Guid Id);
    /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi.  
       Ako u Endpont nema Request object (ako argumente mozemo proslediti kroz URL), ovde mora biti uvek Command object 
    makar i bez argumenata. 
       
       Command/Query object radi sa DTO (ne Entity klasama), zbog Clean architecture, da razdvojim Application 
    layer od Domain layer + Validacija za Command ide u Applicaiton layer da se DTO validira pre nego sto dodje u Domain 
    tj Entity klasu koja je definicija tabele u bazi. Ovo moze i u Vertical slice architecture, ali niko ne radi to. */

    /* U Command(nikad u Query) moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske
     ovo je strongly typed validaton from MediatR. Validacija je ista ona u BuildingBlocks koju Catalog i Basket koristio. */
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator() 
        {
            RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("OrderName is requeired");
            RuleFor(x => x.Order.CustomerId).NotNull().WithMessage("CustomerId is required");
            RuleFor(x => x.Order.OrderItems).NotEmpty().WithMessage("OrderItems ne moze prazno");
            // Samo sam ove kolone zeleo da validiram, a mogo sam i ostale. 
        }
    }

    // Za razliku od Catalog i Basket, Command/QueryHandler se pise u posebnom .cs fajlu, zbog Clean architecture.
}
