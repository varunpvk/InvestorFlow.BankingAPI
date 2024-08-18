using IF.Application.AccountService.CommandHandlers;
using IF.Application.AccountService.Commands;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.AccountServiceTests
{
    public class UpdateAccountCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAccountIsUpdated()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IAccountRepository>();
            mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "test", "test", "test"));
            mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Account>())).ReturnsAsync(true);

            var handler = new UpdateAccountCommandHandler(mockUnitOfWork.Object);
            var command = new UpdateAccountCommand(Guid.NewGuid(), AccountType.Savings, "test", "test");

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
        public async Task Handle_ShouldReturnFailure_WhenAccountUpdationFails()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IAccountRepository>();
            mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "test", "test", "test"));
            mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Account>())).ReturnsAsync(false);

            var handler = new UpdateAccountCommandHandler(mockUnitOfWork.Object);
            var command = new UpdateAccountCommand(Guid.NewGuid(), AccountType.Savings, "test", "test");

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                               success => null,
                               error => error.Message
            );
            Assert.Equal("Failed to update account", errorMessage);
        }
    }
}
