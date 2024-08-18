using IF.Domain;
using IF.Domain.ErrorMessages;

namespace IF.Application.Abstractions
{
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand
    {
        Task<TResult> HandleAsync(TCommand command);
    }
}
