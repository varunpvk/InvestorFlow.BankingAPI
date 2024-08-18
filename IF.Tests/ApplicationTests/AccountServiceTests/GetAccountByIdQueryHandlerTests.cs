using IF.Application.AccountService.Queries;
using IF.Application.AccountService.QueryHandler;
using IF.Domain.DTOs;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.AccountServiceTests
{
    public class GetAccountByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnAccount_WithValidGetAccountByIdQuery()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IAccountRepository>();
            mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new AccountDTO(id.ToString(), "test", "test", "test"));

            var handler = new GetAccountByIdQueryHandler(mockUnitOfWork.Object);
            var query = new GetAccountByIdQuery(id);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WithInvalidGetAccountByIdQuery()
        {
            // Arrange
            var id = Guid.Empty;
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IAccountRepository>();
            mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync(It.IsAny<AccountDTO>());

            var handler = new GetAccountByIdQueryHandler(mockUnitOfWork.Object);
            var query = new GetAccountByIdQuery(id);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            var errorMessage = result.Match(
                                     success => null,
                                     error => error.Message);
            Assert.Equal("Account not found", errorMessage);
        }
    }
}
