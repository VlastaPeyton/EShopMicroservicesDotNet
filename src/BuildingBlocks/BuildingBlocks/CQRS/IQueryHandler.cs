
using MediatR;

namespace BuildingBlocks.CQRS
{
    /* Nema IQueryHandler bez Response, kao ICommandHandler sto ima, jer prilikom citanja iz baze, uvek moramo imati povratnu vrednost. */
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>
                                                                                              where TResponse : notnull  { }
    // IRequestHandler je iz MediatR
}
