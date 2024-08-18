using IF.Application.BankingService.CommandHandlers;
using IF.Application.BankingService.Commands;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;
using FluentAssertions;

namespace IF.Tests.ApplicationTests.BankingServiceTests
{
    public class CreateBankAccountCommandHandlerTests
    {
        [Fact]
        public async Task CreateBankAccountCommandHandler_WhenValid_ShouldCreateBankAccount()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccountsRepository = new Mock<ICustomerAccountRepository>();
            var mockAccountsRepository = new Mock<IAccountRepository>();
            var mockVaultsRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(o => o.CustomerAccounts).Returns(mockCustomerAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Accounts).Returns(mockAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Vaults).Returns(mockVaultsRepository.Object);

            mockCustomerAccountsRepository.Setup(o => o.GetByCustomerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<CustomerAccountDTO>
                {
                    new CustomerAccountDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    }
                });

            mockAccountsRepository.Setup(o => o.AddAsync(It.IsAny<Account>()))
                .ReturnsAsync(true);

            mockVaultsRepository.Setup(o => o.AddAsync(It.IsAny<Vault>()))
                .ReturnsAsync(true);

            mockCustomerAccountsRepository.Setup(o => o.AddAsync(It.IsAny<CustomerAccount>()))
                .ReturnsAsync(true);

            var handler = new CreateBankAccountCommandHandler(mockUnitOfWork.Object);

            // Act
            var result = await handler.HandleAsync(new CreateBankAccountCommand(Guid.NewGuid(), AccountType.Savings, DateTime.UtcNow));

            // Assert
            var success = result.Match(
                               success => success,
                               error => false);

            Assert.True(success);
        }

        [Fact]
        public async Task CreateBankAccountCommandHandler_WhenAccountCreationFails_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccountsRepository = new Mock<ICustomerAccountRepository>();
            var mockAccountsRepository = new Mock<IAccountRepository>();
            var mockVaultsRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(o => o.CustomerAccounts).Returns(mockCustomerAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Accounts).Returns(mockAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Vaults).Returns(mockVaultsRepository.Object);

            mockCustomerAccountsRepository.Setup(o => o.GetByCustomerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<CustomerAccountDTO>
                {
                    new CustomerAccountDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    }
                });

            mockAccountsRepository.Setup(o => o.AddAsync(It.IsAny<Account>()))
                .ReturnsAsync(false);

            var handler = new CreateBankAccountCommandHandler(mockUnitOfWork.Object);

            // Act
            var result = await handler.HandleAsync(new CreateBankAccountCommand(Guid.NewGuid(), AccountType.Savings, DateTime.UtcNow));

            // Assert
            var errorMessage = result.Match(
                                 success => null,
                                 error => error.Message);

            errorMessage.Should().Be("Failed to create account");
        }

        [Fact]
        public async Task CreateBankAccountCommandHandler_WhenVaultCreationFails_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccountsRepository = new Mock<ICustomerAccountRepository>();
            var mockAccountsRepository = new Mock<IAccountRepository>();
            var mockVaultsRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(o => o.CustomerAccounts).Returns(mockCustomerAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Accounts).Returns(mockAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Vaults).Returns(mockVaultsRepository.Object);

            mockCustomerAccountsRepository.Setup(o => o.GetByCustomerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<CustomerAccountDTO>
                {
                    new CustomerAccountDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    }
                });

            mockAccountsRepository.Setup(o => o.AddAsync(It.IsAny<Account>()))
                .ReturnsAsync(true);

            mockVaultsRepository.Setup(o => o.AddAsync(It.IsAny<Vault>()))
                .ReturnsAsync(false);

            var handler = new CreateBankAccountCommandHandler(mockUnitOfWork.Object);

            // Act
            var result = await handler.HandleAsync(new CreateBankAccountCommand(Guid.NewGuid(), AccountType.Savings, DateTime.UtcNow));

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);

            errorMessage.Should().Be("Failed to create vault");
        }

        [Fact]
        public async Task CreateBankAccountCommandHandler_WhenCustomerAccountCreationFails_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccountsRepository = new Mock<ICustomerAccountRepository>();
            var mockAccountsRepository = new Mock<IAccountRepository>();
            var mockVaultsRepository = new Mock<IVaultRepository>();
            mockUnitOfWork.Setup(o => o.CustomerAccounts).Returns(mockCustomerAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Accounts).Returns(mockAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Vaults).Returns(mockVaultsRepository.Object);

            mockCustomerAccountsRepository.Setup(o => o.GetByCustomerIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<CustomerAccountDTO>
                {
                    new CustomerAccountDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    }
                });

            mockAccountsRepository.Setup(o => o.AddAsync(It.IsAny<Account>()))
                .ReturnsAsync(true);

            mockVaultsRepository.Setup(o => o.AddAsync(It.IsAny<Vault>()))
                .ReturnsAsync(true);

            mockCustomerAccountsRepository.Setup(o => o.AddAsync(It.IsAny<CustomerAccount>()))
                .ReturnsAsync(false);

            var handler = new CreateBankAccountCommandHandler(mockUnitOfWork.Object);

            // Act
            var result = await handler.HandleAsync(new CreateBankAccountCommand(Guid.NewGuid(), AccountType.Savings, DateTime.UtcNow));

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);

            errorMessage.Should().Be("Failed to create customer account");
        }
    }
}
