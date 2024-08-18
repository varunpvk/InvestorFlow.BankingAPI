using IF.Application.TransactionService.Queries;
using IF.Application.TransactionService.QueryHandler;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Domain.ValueObjects;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.TransactionServiceTests
{
    public class GetTransactionByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnTransaction_WhenTransactionExists()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(uow => uow.Transactions).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(new TransactionDTO(
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                TransactionType.Credit.ToString(),
                100,
                "GBP",
                "Test",
                DateTime.UtcNow));

            var handler = new GetTransactionByIdQueryHandler(mockUnitOfWork.Object);
            var query = new GetTransactionByIdQuery(Guid.NewGuid());

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            var transaction = result.Match(
                               success => success,
                               error => null);

            Assert.NotNull(transaction);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTransactionDoesNotExist()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(uow => uow.Transactions).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((TransactionDTO)null);

            var handler = new GetTransactionByIdQueryHandler(mockUnitOfWork.Object);
            var query = new GetTransactionByIdQuery(Guid.NewGuid());

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            var errorMessage = result.Match(
                                     success => null,
                                     error => error.Message);

            Assert.Equal("Transaction not found", errorMessage);
        }
    }
}
