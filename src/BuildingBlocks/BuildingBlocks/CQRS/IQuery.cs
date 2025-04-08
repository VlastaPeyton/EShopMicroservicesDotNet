using MediatR;

namespace BuildingBlocks.CQRS
{
    /* Nema IQuery bez <out TResponse>, kao ICommand sto ima, jer Query je za
    citanje iz baze, a tad uvek ima povratna vrednost. */
    public interface IQuery<out TResponse> : IRequest<TResponse>
        where TResponse : notnull
    {
    }
    /* IRequest je iz MediatR.
       IQuery : IRequest, jer se Request object from Endpoint.cs mapira u Query object u Handler.cs 
       IQuery, jer je dobra praksa da Query object imeplementira ovaj interface. */
}
