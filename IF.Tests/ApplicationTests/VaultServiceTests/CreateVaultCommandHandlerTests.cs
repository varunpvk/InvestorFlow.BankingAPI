using IF.Application.VaultService.CommandHandlers;
using IF.Application.VaultService.Commands;
using IF.Domain.Entities;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.VaultServiceTests
{
    public class CreateVaultCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenVaultIsCreated()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(uow => uow.Vaults).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Vault>())).ReturnsAsync(true);

            var handler = new CreateVaultCommandHandler(mockUnitOfWork.Object);
            var command = new CreateVaultCommand(Guid.NewGuid(), Guid.NewGuid());

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
        public async Task Handle_ShouldReturnFailure_WhenVaultCreationFails()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(uow => uow.Vaults).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Vault>())).ReturnsAsync(false);

            var handler = new CreateVaultCommandHandler(mockUnitOfWork.Object);
            var command = new CreateVaultCommand(Guid.NewGuid(), Guid.NewGuid());

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                               success => null,
                               error => error.Message
            );
            Assert.Equal("Failed to create vault", errorMessage);
        }
    }
}
