using IF.Application.VaultService.Queries;
using IF.Application.VaultService.QueryHandler;
using IF.Domain.DTOs;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.VaultServiceTests
{
    public class GetVaultByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnVault_WithValidGetVaultByIdQuery()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(uow => uow.Vaults).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(id.ToString(), "test", 20, "test"));

            var handler = new GetVaultByIdQueryHandler(mockUnitOfWork.Object);
            var query = new GetVaultByIdQuery(id);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Handle_ShouldReturnError_WithInvalidGetVaultByIdQuery()
        {
            // Arrange
            var id = Guid.Empty;
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(uow => uow.Vaults).Returns(mockRepository.Object);
            mockRepository.Setup(repo => repo.GetAsync(It.IsAny<Guid>())).ReturnsAsync(It.IsAny<VaultDTO>());

            var handler = new GetVaultByIdQueryHandler(mockUnitOfWork.Object);
            var query = new GetVaultByIdQuery(id);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            var errorMessage = result.Match(
                                     success => null,
                                     error => error.Message);
            Assert.Equal("Vault not found", errorMessage);
        }
    }
}
