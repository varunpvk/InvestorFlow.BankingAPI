using IF.Application.Abstractions;
using IF.Application.TransactionService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.TransactionService.QueryHandler
{
    public class GetTransactionsByAccountIdQueryHandler : IQueryHandler<GetTransactionsByAccountIdQuery, Result<IList<TransactionDTO>, NotFoundError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetTransactionsByAccountIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IList<TransactionDTO>, NotFoundError>> HandleAsync(GetTransactionsByAccountIdQuery query)
        {
            try
            {
                var transactions = await _unitOfWork.Transactions.GetByAccountIdAsync(query.AccountId);

                if(transactions.Any())
                {
                    return Result<IList<TransactionDTO>, NotFoundError>.Success(transactions.ToList());
                }

                return Result<IList<TransactionDTO>, NotFoundError>.Failure(new NotFoundError("Transactions not found"));
            }
            catch (Exception)
            {
                return Result<IList<TransactionDTO>, NotFoundError>.Failure(new NotFoundError("Exception while getting transactions"));
            }
        }
    }
}
