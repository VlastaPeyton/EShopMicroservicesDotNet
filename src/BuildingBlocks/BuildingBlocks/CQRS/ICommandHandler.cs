using MediatR;

namespace BuildingBlocks.CQRS
{
    /*ICommandHandler without return Response object, jer imamo Endpoint/Handler gde ima samo Command bez Response object, jer prilikom upisa(modifikacije) baze, ne moramo uvek imati povratnu vrednost.
    Ovo mozer jer u ICommand.cs imamo slucaj kad nema TResponse. */
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit> where TCommand : ICommand<Unit> { }
    // Unit is void type for MediatR
    
    /* ICommandHandler with return TResponse object jer imamo ICommand sa Response definisan u ICommand.cs */
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse> 
                                                                                                    where TResponse : notnull { }
    // IRequestHandler je iz MediatR
}
