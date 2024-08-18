using IF.Application.AccountService.CommandHandlers;
using IF.Application.AccountService.Commands;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.AccountServiceTests
{
    public class DeleteAccountCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenAccountIsDeleted()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IAccountRepository>();
            mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var handler = new DeleteAccountCommandHandler(mockUnitOfWork.Object);
            var command = new DeleteAccountCommand(Guid.NewGuid());

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
        public async Task Handle_ShouldReturnFailure_WhenAccountDeletionFails()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IAccountRepository>();
            mockUnitOfWork.Setup(uow => uow.Accounts).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var handler = new DeleteAccountCommandHandler(mockUnitOfWork.Object);
            var command = new DeleteAccountCommand(Guid.NewGuid());

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                               success => null,
                               error => error.Message
            );
            Assert.Equal("Failed to delete account", errorMessage);
        }
    }
}
