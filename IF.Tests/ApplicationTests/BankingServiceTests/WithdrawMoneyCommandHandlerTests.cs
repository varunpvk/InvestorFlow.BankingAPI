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
    public class WithdrawMoneyCommandHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenWithdrawMoneyIsSuccessful_ShouldReturnSuccess()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<WithdrawMoneyCommandHandler>>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var customerAccounts = new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString()
                }
            };
            
            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(customerAccounts);
            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new AccountDTO(Guid.NewGuid().ToString(), AccountType.Savings.ToString(), "test", "test"));
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(true);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(true);

            var command = new WithdrawMoneyCommand(Guid.NewGuid(), 100, "USD", AccountType.Savings);

            var handler = new WithdrawMoneyCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var success = result.Match(
                success => success,
                error => false
                );
            Assert.True(success);
        }

        [Fact]
        public async Task HandleAsync_WhenWithdrawMoneyFailsToUpdateVault_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<WithdrawMoneyCommandHandler>>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var customerAccounts = new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString()
                }
            };
            
            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(customerAccounts);
            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                                   new AccountDTO(Guid.NewGuid().ToString(), AccountType.Savings.ToString(), "test", "test"));
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                                   new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(false);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(true);

            var command = new WithdrawMoneyCommand(Guid.NewGuid(), 100, "USD", AccountType.Savings);

            var handler = new WithdrawMoneyCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMesage = result.Match(
                               success => null,
                               error => error.Message);
            Assert.Equal("Failed to update vault", errorMesage);
        }

        [Fact]
        public async Task HandleAsync_WhenWithdrawMoneyFailsToAddTransaction_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<WithdrawMoneyCommandHandler>>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var customerAccounts = new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString()
                }
            };
            
            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(customerAccounts);
            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), AccountType.Savings.ToString(), "test", "test"));
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(true);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(false);

            var command = new WithdrawMoneyCommand(Guid.NewGuid(), 100, "USD", AccountType.Savings);

            var handler = new WithdrawMoneyCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMesage = result.Match(
                                     success => null,
                                     error => error.Message);

            Assert.Equal("Failed to add transaction", errorMesage);
        }

        [Fact]
        public async Task HandleAsync_WhenWithdrawMoneyFailsToGetAccount_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<WithdrawMoneyCommandHandler>>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var customerAccounts = new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString()
                }
            };
            
            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(customerAccounts);
            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync((AccountDTO)null);

            var command = new WithdrawMoneyCommand(Guid.NewGuid(), 100, "USD", AccountType.Savings);

            var handler = new WithdrawMoneyCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMesage = result.Match(
                                     success => null,
                                     error => error.Message);

            Assert.Equal("Failed to withdraw money", errorMesage);
        }
    }
}
