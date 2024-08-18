using IF.Application.AccountService.CommandHandlers;
using IF.Application.AccountService.Commands;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.AccountServiceTests
{
    public class CreateAccountCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAccountIsCreated()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IAccountRepository>();
            mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Account>())).ReturnsAsync(true);

            var handler = new CreateAccountCommandHandler(mockUnitOfWork.Object);
            var command = new CreateAccountCommand(Guid.NewGuid(), AccountType.Savings);

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
        public async Task Handle_ShouldReturnFailure_WhenAccountCreationFails()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IAccountRepository>();
            mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Account>())).ReturnsAsync(false);

            var handler = new CreateAccountCommandHandler(mockUnitOfWork.Object);
            var command = new CreateAccountCommand(Guid.NewGuid(), AccountType.Savings);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                               success => null,
                               error => error.Message
            );
            Assert.Equal("Failed to create account", errorMessage);
        }

        // Add unit tests for Validation Error Messages for CreateAccountCommandHandler
    }
}
