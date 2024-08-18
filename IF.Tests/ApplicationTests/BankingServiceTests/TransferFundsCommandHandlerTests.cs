using FluentAssertions;
using IF.Application.BankingService.CommandHandlers;
using IF.Application.BankingService.Commands;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.Enums;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Moq;

namespace IF.Tests.ApplicationTests.BankingServiceTests
{
    public class TransferFundsCommandHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenTransferFundsIsSuccessfull_ShouldReturnSuccess()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var command = new TransferFundsCommand(Guid.NewGuid(), AccountType.Savings, Guid.NewGuid(), 100, "USD");

            var handler = new TransferFundsCommandHandler(mockUnitOfWork.Object);

            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString()
                }
            });

            mockCustomerAccounts.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new CustomerAccountDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    });

            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(true);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(true);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var success = result.Match(
                                success => success,
                                error => false); 
            Assert.True(success);
        }

        [Fact]
        public async Task HandleAsync_WhenTransferFundsIsUnSuccessfull_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var command = new TransferFundsCommand(Guid.NewGuid(), AccountType.Savings, Guid.NewGuid(), 100, "USD");

            var handler = new TransferFundsCommandHandler(mockUnitOfWork.Object);

            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString()
                }
            });

            mockCustomerAccounts.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                                   new CustomerAccountDTO
                                   {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    });

            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(false);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(false);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                                 success => null,
                                 error => error.Message); 
            errorMessage.Should().Be("Failed to transfer funds");
        }

        [Fact]
        public async Task HandleAsync_WhenTransferFundsIsUnSuccessfull_ShouldReturnFailureForTransaction()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var command = new TransferFundsCommand(Guid.NewGuid(), AccountType.Savings, Guid.NewGuid(), 100, "USD");

            var handler = new TransferFundsCommandHandler(mockUnitOfWork.Object);

            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString()
                }
            });

            mockCustomerAccounts.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new CustomerAccountDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    });

            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(true);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(false);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message); 

            errorMessage.Should().Be("Failed to add transaction");
        }

        [Fact]
        public async Task HandleAsync_WhenTransferFundsIsUnSuccessfull_ShouldReturnFailureForSourceVault()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var command = new TransferFundsCommand(Guid.NewGuid(), AccountType.Savings, Guid.NewGuid(), 100, "USD");

            var handler = new TransferFundsCommandHandler(mockUnitOfWork.Object);

            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString()
                }
            });

            mockCustomerAccounts.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new CustomerAccountDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    });

            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(false);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(true);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message); 

            errorMessage.Should().Be("Failed to transfer funds");
        }

        [Fact]
        public async Task HandleAsync_WhenTransferFundsIsUnSuccessfull_ShouldReturnFailureForDestinationVault()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var command = new TransferFundsCommand(Guid.NewGuid(), AccountType.Savings, Guid.NewGuid(), 100, "USD");

            var handler = new TransferFundsCommandHandler(mockUnitOfWork.Object);

            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString()
                }
            });

            mockCustomerAccounts.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                    new CustomerAccountDTO
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    });

            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(false);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(true);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message); 

            errorMessage.Should().Be("Failed to transfer funds");
        }

        [Fact]
        public async Task HandleAsync_WhenTransferFundsIsUnSuccessfull_ShouldReturnFailureForSourceAccount()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockAccounts = new Mock<IAccountRepository>();
            var mockVaults = new Mock<IVaultRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);
            mockUnitOfWork.Setup(x => x.Accounts).Returns(mockAccounts.Object);
            mockUnitOfWork.Setup(x => x.Vaults).Returns(mockVaults.Object);
            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);

            var command = new TransferFundsCommand(Guid.NewGuid(), AccountType.Savings, Guid.NewGuid(), 100, "USD");

            var handler = new TransferFundsCommandHandler(mockUnitOfWork.Object);

            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString()
                }
            });

            mockCustomerAccounts.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(
                                   new CustomerAccountDTO
                                   {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                    });

            mockAccounts.Setup(x => x.GetAsync(It.IsAny<Guid>())).ReturnsAsync((AccountDTO)null);
            mockVaults.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 100, "USD"));
            mockVaults.Setup(x => x.UpdateAsync(It.IsAny<Vault>())).ReturnsAsync(true);
            mockTransactions.Setup(x => x.AddAsync(It.IsAny<Transaction>())).ReturnsAsync(true);

            // Act
            var result = await handler.HandleAsync(command);

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message); 

            errorMessage.Should().Be("Failed to transfer funds");
        }
    }
}
