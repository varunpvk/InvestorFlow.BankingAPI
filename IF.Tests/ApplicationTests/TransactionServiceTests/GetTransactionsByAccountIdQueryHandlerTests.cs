using IF.Application.TransactionService.Queries;
using IF.Application.TransactionService.QueryHandler;
using IF.Domain.DTOs;
using IF.Domain.Enums;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.TransactionServiceTests
{
    public class GetTransactionsByAccountIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnTransactions_WhenTransactionsExist()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(uow => uow.Transactions).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<TransactionDTO>
            {
                new TransactionDTO(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    TransactionType.Credit.ToString(),
                    100,
                    "GBP",
                    "Test",
                    DateTime.UtcNow),
                new TransactionDTO(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    TransactionType.Debit.ToString(),
                    50,
                    "GBP",
                    "Test",
                    DateTime.UtcNow)
            });

            var handler = new GetTransactionsByAccountIdQueryHandler(mockUnitOfWork.Object);
            var query = new GetTransactionsByAccountIdQuery(Guid.NewGuid());

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            var transactions = result.Match(
                               success => success,
                               error => null);
            Assert.NotNull(transactions);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTransactionsDoNotExist()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(uow => uow.Transactions).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<TransactionDTO>());

            var handler = new GetTransactionsByAccountIdQueryHandler(mockUnitOfWork.Object);
            var query = new GetTransactionsByAccountIdQuery(Guid.NewGuid());

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);
            Assert.Equal("Transactions not found", errorMessage);
        }
    }
}
