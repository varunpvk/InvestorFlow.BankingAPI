using IF.Application.AccountService.CommandHandlers;
using IF.Application.AccountService.Commands;
using IF.Application.VaultService.CommandHandlers;
using IF.Application.VaultService.Commands;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.VaultServiceTests
{
    public class DeleteVaultCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenVaultIsDeleted()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(uow => uow.Vaults).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var handler = new DeleteVaultCommandHandler(mockUnitOfWork.Object);
            var command = new DeleteVaultCommand(Guid.NewGuid());

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
        public async Task Handle_ShouldReturnFailure_WhenVaultDeletionFails()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(uow => uow.Vaults).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var handler = new DeleteVaultCommandHandler(mockUnitOfWork.Object);
            var command = new DeleteVaultCommand(Guid.NewGuid());

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                               success => null,
                               error => error.Message
            );
            Assert.Equal("Failed to delete vault", errorMessage);
        }
    }
}
