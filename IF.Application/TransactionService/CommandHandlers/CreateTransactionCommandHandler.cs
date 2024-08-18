using IF.Application.Abstractions;
using IF.Application.TransactionService.Commands;
using IF.Domain;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.TransactionService.CommandHandlers
{
    public class CreateTransactionCommandHandler : ICommandHandler<CreateTransactionCommand, Result<bool, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTransactionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool, ValidationError>> HandleAsync(CreateTransactionCommand command)
        {
            try
            {
                var transaction = new Transaction(
                    command.Id,
                    command.CustomerAccountId,
                    command.Details,
                    command.TransactionDateUtc);

                _unitOfWork.BeginTransaction();
                var success = await _unitOfWork.Transactions.AddAsync(transaction);

                if (success)
                {
                    _unitOfWork.Commit();
                    return Result<bool, ValidationError>.Success(true);
                }

                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError("Failed to create transaction"));
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return Result<bool, ValidationError>.Failure(new ValidationError($"Exception while creating transaction: {ex.Message}"));
            }
        }
    }
}
