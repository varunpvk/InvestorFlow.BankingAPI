using IF.Application.TransactionService.CommandHandlers;
using IF.Application.TransactionService.Commands;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Domain.ValueObjects;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.TransactionServiceTests
{
    public class CreateTransactionCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTransactionIsCreated()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(uow => uow.Transactions).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(true);

            var handler = new CreateTransactionCommandHandler(mockUnitOfWork.Object);
            var command = new CreateTransactionCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new TransactionDetails(
                    TransactionType.Withdrawal,
                    100,
                    "GBP"
                ),
                DateTime.UtcNow);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var isSuccess = result.Match(
                               success => success,
                                              error => false
                                                         );
            Assert.True(isSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenTransactionCreationFails()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(uow => uow.Transactions).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(false);

            var handler = new CreateTransactionCommandHandler(mockUnitOfWork.Object);
            var command = new CreateTransactionCommand(
                               Guid.NewGuid(),
                               Guid.NewGuid(),
                               new TransactionDetails(
                                              TransactionType.Withdrawal,
                                              100,
                                              "GBP"),
                               DateTime.UtcNow);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message
                                      );

            Assert.Equal("Failed to create transaction", errorMessage);
        }
    }
}
