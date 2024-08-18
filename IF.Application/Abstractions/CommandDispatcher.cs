using Microsoft.Extensions.DependencyInjection;

namespace IF.Application.Abstractions
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command)
            where TCommand : ICommand
        {
            var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();

            return await handler.HandleAsync(command);
        }

        public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query)
            where TQuery : IQuery<TResult>
        {
            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

            return await handler.HandleAsync(query);
        }
    }
}
