
using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Ordering.Application.DTOs;

namespace Ordering.Application.Orders.Queries.GetOrders
{   /* Prvo pogledaj GetOrdersByCustomerEndpoint.cs u API layer i onu sliku da bi razume ovo lakse, jer iz Endpoint mapira
    se ovde */
    public record GetOrdersQuery(PaginationRequest paginationRequest) : IQuery<GetOrdersResult>;
    public record GetOrdersResult(PaginatedResult<OrderDTO> Orders);

   /* Command/Query i Result object mora imati argumente istog imeta i tipa kao Request i Response object u 
   Endpoint klasi, respektivno, kako bi mapiranje moglo da se izvrsi. 
        Ako u Endpont nema Request object (ako argumente mozemo proslediti kroz URL), ovde mora biti uvek Query object 
   makar i bez argumenata. 

       Command/Query object radi sa DTO (ne Entity klasama), zbog Clean architecture, da razdvojim Application 
   layer od Domain layer. Ovo moze i u Vertical slice architecture, ali niko ne radi to. 

   Nema validacija jer Query nema to nikad jer cita iz baze samo. */
}
