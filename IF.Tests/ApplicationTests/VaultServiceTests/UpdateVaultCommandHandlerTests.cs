using IF.Application.VaultService.CommandHandlers;
using IF.Application.VaultService.Commands;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.VaultServiceTests
{
    public class UpdateVaultCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenVaultIsUpdated()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(uow => uow.Vaults).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 20, "test"));
            mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(true);

            var handler = new UpdateVaultCommandHandler(mockUnitOfWork.Object);
            var command = new UpdateVaultCommand(Guid.NewGuid(), 20, "test");

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
        public async Task Handle_ShouldReturnFailure_WhenVaultUpdationFails()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(uow => uow.Vaults).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 20, "test"));
            mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(false);

            var handler = new UpdateVaultCommandHandler(mockUnitOfWork.Object);
            var command = new UpdateVaultCommand(Guid.NewGuid(), 20, "test");

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                               success => null,
                               error => error.Message
            );
            Assert.Equal("Failed to update vault", errorMessage);
        }
    }
}
