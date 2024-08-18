using IF.Application.Abstractions;
using IF.Application.BankingService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using IF.Infrastructure.BankingRepository;
using Microsoft.Extensions.Logging;

namespace IF.Application.BankingService.QueryHandler
{
    public class GetTransactionHistoryQueryHandler : IQueryHandler<GetTransactionHistoryQuery, Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetTransactionHistoryQueryHandler> _logger;

        public GetTransactionHistoryQueryHandler(IUnitOfWork unitOfWork, ILogger<GetTransactionHistoryQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
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
                    _logger.LogInformation("Transaction history found");
                    return Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>.Success(transactionHistory);
                }

                _logger.LogError("Transaction history not found");
                return Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>.Failure(new NotFoundError("Transaction history not found"));

            }
            catch (Exception)
            {
                _logger.LogError("Exception while getting transaction history");
                return Result<IDictionary<Guid, IList<TransactionDTO>>, NotFoundError>.Failure(new NotFoundError("Exception while getting transaction history"));
            }
        }
    }
}
