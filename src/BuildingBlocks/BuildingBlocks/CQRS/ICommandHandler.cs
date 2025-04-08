using MediatR;

namespace BuildingBlocks.CQRS
{
    /*ICommandHandler without return object Response jer imamo ICommand i bez Response
    definsian u ICommand.cs, jer prilikom upisa(modifikacije) baze, ne moramo uvek imati 
    povratnu vrednost.*/
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
        where TCommand : ICommand<Unit>
    {   // Unit is void type for MediatR
    }

    /* ICommandHandler with return object Response jer imamo ICommand  sa Response 
    definisan u ICommand.cs */
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : notnull
    { }
    // IRequestHandler je iz MediatR
}
