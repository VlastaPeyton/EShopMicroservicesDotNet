using MediatR;

namespace BuildingBlocks.CQRS
{
    // Nema IQuery bez <out TResponse>, kao ICommand sto ima, jer Query je za citanje iz baze, a tad uvek ima povratna vrednost. I zato mora where TResponse: notnull
    public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull {} // Zbog ovoga moze GetProductsQuery(...) : IQuery<GetProductsResult>
    /* IRequest je iz MediatR.
       IQuery : IRequest, jer se Request object in Endpoint.cs mapira u Query object. 
       IQuery, jer je dobra praksa da Query object imeplementira ovaj interface. */
}
