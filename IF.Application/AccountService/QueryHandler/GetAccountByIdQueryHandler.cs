using IF.Application.Abstractions;
using IF.Application.AccountService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.ErrorMessages;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;

namespace IF.Application.AccountService.QueryHandler
{
    public class GetAccountByIdQueryHandler : IQueryHandler<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAccountByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AccountDTO, NotFoundError>> HandleAsync(GetAccountByIdQuery query)
        {
            try
            {
                var account = await _unitOfWork.Accounts.GetAsync(query.Id);

                if (account != null)
                {
                    return Result<AccountDTO, NotFoundError>.Success(account);
                }
                
                return Result<AccountDTO, NotFoundError>.Failure(new NotFoundError("Account not found"));
            }
            catch (Exception ex)
            {
                return Result<AccountDTO, NotFoundError>.Failure(new NotFoundError($"Exception while getting account: {ex.Message}"));
            }
            
        }
    }
}
