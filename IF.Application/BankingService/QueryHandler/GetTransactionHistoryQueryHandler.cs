using IF.Application.Abstractions;
using IF.Application.BankingService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.BankingService.QueryHandler
{
    public class GetTransactionHistoryQueryHandler : IQueryHandler<GetTransactionHistoryQuery, Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTransactionHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>> HandleAsync(GetTransactionHistoryQuery query)
        {
            try
            {
                //Get List of Accounts associated with the customer.
                var customerAccountDTOs = await _unitOfWork.CustomerAccounts.GetByCustomerIdAsync(query.CustomerId);
                IDictionary<Guid, IList<TransactionDTO>> transactionHistory = new Dictionary<Guid, IList<TransactionDTO>>();

                foreach(var customerAccountDTO in customerAccountDTOs)
                {
                    var transactions = await _unitOfWork.Transactions.GetByAccountIdAsync(Guid.Parse(customerAccountDTO.Id));
                    transactionHistory.Add(Guid.Parse(customerAccountDTO.AccountId), transactions.ToList());
                }

                if(transactionHistory.Any())
                {
                    return Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>.Success(transactionHistory);
                }

                return Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>.Failure(new NotFoundError("Transaction history not found"));

            }
            catch (Exception)
            {
                return Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>.Failure(new NotFoundError("Exception while getting transaction history"));
            }
        }
    }
}
