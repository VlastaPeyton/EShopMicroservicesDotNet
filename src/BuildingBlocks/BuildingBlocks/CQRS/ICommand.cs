
using MediatR;

namespace BuildingBlocks.CQRS
{
    // ICommand is for write operations in DB

    /* ICommand without Response, jer nekad nece trebati Response
     tj povratna vrednost kada nesto upisemo u bazu.*/
    public interface ICommand : ICommand<Unit> { }
    // Unit je void za MediatR kad ne vraca  Response 

    // ICommand with Response
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
    /* IRequest je iz MediatR

    /* ICommand : IRequest jer se Request object iz Endpoint.cs mapira u Command object u Handler.cs
       Pravimo ICommand, jer je to dobra praksa da bi sva Command object u Handler klasi 
    implementirala ovo.*/
}
