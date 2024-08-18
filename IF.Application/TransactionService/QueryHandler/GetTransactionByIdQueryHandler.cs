using IF.Application.Abstractions;
using IF.Application.TransactionService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.TransactionService.QueryHandler
{
    public class GetTransactionByIdQueryHandler : IQueryHandler<GetTransactionByIdQuery, Result<TransactionDTO, NotFoundError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTransactionByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<TransactionDTO, NotFoundError>> HandleAsync(GetTransactionByIdQuery query)
        {
            try
            {
                var transaction = await _unitOfWork.Transactions.GetByIdAsync(query.Id);

                if (transaction != null)
                {
                    return Result<TransactionDTO, NotFoundError>.Success(transaction);
                }

                return Result<TransactionDTO, NotFoundError>.Failure(new NotFoundError("Transaction not found"));
            }
            catch (Exception ex)
            {
                return Result<TransactionDTO, NotFoundError>.Failure(new NotFoundError($"Exception while getting transaction: {ex.Message}"));
            }
        }
    }
}
