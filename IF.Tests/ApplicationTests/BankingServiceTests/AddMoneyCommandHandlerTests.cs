using IF.Application.BankingService.CommandHandlers;
using IF.Application.BankingService.Commands;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace IF.Tests.ApplicationTests.BankingServiceTests
{
    public class AddMoneyCommandHandlerTests
    {
        [Fact]
        public async Task AddMoneyCommandHandler_WhenValidAmount_ShouldAddMoney()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<AddMoneyCommandHandler>>();
            var mockCustomerAccountsRepository = new Mock<ICustomerAccountRepository>();
            var mockAccountsRepository = new Mock<IAccountRepository>();
            var mockVaultsRepository = new Mock<IVaultRepository>();
            var mockTransactionsRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(o => o.CustomerAccounts).Returns(mockCustomerAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Accounts).Returns(mockAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Vaults).Returns(mockVaultsRepository.Object);
            mockUnitOfWork.Setup(o => o.Transactions).Returns(mockTransactionsRepository.Object);

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

            mockAccountsRepository.Setup(o => o.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "123456", "12345678"));

            mockVaultsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1000, "GBP"));

            mockVaultsRepository.Setup(o => o.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(true);
            
            mockTransactionsRepository.Setup(o => o.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(true);

            mockCustomerAccountsRepository.Setup(o => o.AddAsync(It.IsAny<CustomerAccount>()))
                .ReturnsAsync(true);

            var handler = new AddMoneyCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new AddMoneyCommand(Guid.NewGuid(), 100, "GBP", AccountType.Savings));

            // Assert
            var success = result.Match(
                               success => success,
                               error => false);

            Assert.True(success);
        }

        [Fact]
        public async Task AddMoneyCommandHandler_WhenFailedToUpdateVault_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<AddMoneyCommandHandler>>();
            var mockCustomerAccountsRepository = new Mock<ICustomerAccountRepository>();
            var mockAccountsRepository = new Mock<IAccountRepository>();
            var mockVaultsRepository = new Mock<IVaultRepository>();
            var mockTransactionsRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(o => o.CustomerAccounts).Returns(mockCustomerAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Accounts).Returns(mockAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Vaults).Returns(mockVaultsRepository.Object);
            mockUnitOfWork.Setup(o => o.Transactions).Returns(mockTransactionsRepository.Object);

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

            mockAccountsRepository.Setup(o => o.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "123456", "12345678"));

            mockVaultsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1000, "GBP"));

            mockVaultsRepository.Setup(o => o.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(false);
            
            mockTransactionsRepository.Setup(o => o.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(true);

            var handler = new AddMoneyCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new AddMoneyCommand(Guid.NewGuid(), 100, "GBP", AccountType.Savings));

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);

            Assert.Equal("Failed to update vault", errorMessage);
        }

        [Fact]
        public async Task AddMoneyCommandHandler_WhenFailedToAddTransaction_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<AddMoneyCommandHandler>>();
            var mockCustomerAccountsRepository = new Mock<ICustomerAccountRepository>();
            var mockAccountsRepository = new Mock<IAccountRepository>();
            var mockVaultsRepository = new Mock<IVaultRepository>();
            var mockTransactionsRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(o => o.CustomerAccounts).Returns(mockCustomerAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Accounts).Returns(mockAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Vaults).Returns(mockVaultsRepository.Object);
            mockUnitOfWork.Setup(o => o.Transactions).Returns(mockTransactionsRepository.Object);

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

            mockAccountsRepository.Setup(o => o.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "123456", "12345678"));

            mockVaultsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1000, "GBP"));

            mockVaultsRepository.Setup(o => o.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(true);
            
            mockTransactionsRepository.Setup(o => o.AddAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(false);

            var handler = new AddMoneyCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new AddMoneyCommand(Guid.NewGuid(), 100, "GBP", AccountType.Savings));

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);

            Assert.Equal("Failed to add transaction", errorMessage);
        }

        [Fact]
        public async Task AddMoneyCommandHandler_WhenFailedToGetAccount_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<AddMoneyCommandHandler>>();
            var mockCustomerAccountsRepository = new Mock<ICustomerAccountRepository>();
            var mockAccountsRepository = new Mock<IAccountRepository>();
            var mockVaultsRepository = new Mock<IVaultRepository>();
            var mockTransactionsRepository = new Mock<ITransactionRepository>();
            mockUnitOfWork.Setup(o => o.CustomerAccounts).Returns(mockCustomerAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Accounts).Returns(mockAccountsRepository.Object);
            mockUnitOfWork.Setup(o => o.Vaults).Returns(mockVaultsRepository.Object);
            mockUnitOfWork.Setup(o => o.Transactions).Returns(mockTransactionsRepository.Object);

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

            mockAccountsRepository.Setup(o => o.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync((AccountDTO)null);

            var handler = new AddMoneyCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new AddMoneyCommand(Guid.NewGuid(), 100, "GBP", AccountType.Savings));

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);

            Assert.Equal("Failed to add money", errorMessage);
        }
    }
}
