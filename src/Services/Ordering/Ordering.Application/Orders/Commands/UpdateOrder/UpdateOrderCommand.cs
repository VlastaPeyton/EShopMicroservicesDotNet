
using System.Windows.Input;
using BuildingBlocks.CQRS;
using FluentValidation;
using Ordering.Application.DTOs;
using Ordering.Domain.Models;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{   /* Prvo pogledaj UpdateOrderEndpoint.cs u API layer i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */

    public record UpdateOrderCommand(OrderDTO Order) : ICommand<UpdateOrderResult>;
    public record UpdateOrderResult(bool IsSuccess);
    /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
     Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi. 
        Ako u Endpont nema Request object (ako argumente mozemo proslediti kroz URL), ovde mora biti uvek Command object 
    makar i bez argumenata. 

       Command/Query object radi sa DTO (ne Entity klasama), zbog Clean architecture, da razdvojim Application 
    layer od Domain layer + Validacija za Command ide u Applicaiton layer da se DTO validira pre nego sto dodje u Domain 
    tj Entity klasu koja je definicija tabele u bazi. Ovo moze i u Vertical slice architecture, ali niko ne radi to. */

    /* U Command(nikad u Query) moram da validiram zeljene Command argumente jer se upisuje u bazu, pa da ne dodje do greske
     ovo je strongly typed validaton from MediatR. Validacija je ista ona u BuildingBlocks koju Catalog i Basket koristio. */
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Order.Id).NotEmpty().WithMessage("Id requeired");
            RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("OrderName requeired");
            RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("CustomerId required");
            // Mogo sam jos kolona validirati, ali necu
        }
    }
    // Za razliku od Catalog i Basket, Command/QueryHandler se pise u posebnom .cs fajlu, zbog Clean architecture.

}
