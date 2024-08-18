using IF.Domain;
using IF.Domain.ErrorMessages;

namespace IF.Application.Abstractions
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query);
    }
}
