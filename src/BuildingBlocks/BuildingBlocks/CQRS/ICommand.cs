
using MediatR;

namespace BuildingBlocks.CQRS
{
    // ICommand is for write operations in DB

    /* ICommand without Response, jer nekad nece trebati Response tj povratna vrednost iz baze kada nesto upisemo u bazu.*/
    public interface ICommand : ICommand<Unit> { }
    // Unit je void za MediatR za slucaj kad Handler ne vraca Response to client.

    // ICommand with Response fro DB
    public interface ICommand<out TResponse> : IRequest<TResponse> { } // Zbog ovoga moze CreateProductCommand(...) : ICommand<CreateProductResult> 
    /* IRequest je iz MediatR

    /* ICommand : IRequest jer se Request object u Endpoint.cs mapira u Command object.
       Pravimo ICommand, jer je to dobra praksa da bi sva Command object u Handler klasi implementirala ovaj interface.*/
}
