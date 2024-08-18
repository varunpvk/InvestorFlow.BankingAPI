using FluentAssertions;
using IF.Application.BankingService.CommandHandlers;
using IF.Application.BankingService.Commands;
using IF.Domain.DTOs;
using IF.Domain.Enums;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace IF.Tests.ApplicationTests.BankingServiceTests
{
    public class DeleteBankAccountCommandHandlerTests
    {
        [Fact]
        public async Task DeleteBankAccountCommandHandler_WhenValid_ShouldDeleteBankAccount()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<DeleteBankAccountCommandHandler>>();
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

            mockTransactionsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<TransactionDTO>
                {
                    new TransactionDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), TransactionType.Debit.ToString(), 100, "test", "test", DateTime.Now)
                });

            mockTransactionsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockAccountsRepository.Setup(o => o.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));

            mockAccountsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockVaultsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockVaultsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), "test", 100, "GBP"));

            mockCustomerAccountsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var handler = new DeleteBankAccountCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new DeleteBankAccountCommand(Guid.NewGuid()));

            // Assert
            var success = result.Match(
                                 success => success,
                                 error => false);

            Assert.True(success);
        }

        [Fact]
        public async Task DeleteBankAccountCommandHandler_WhenTransactionDeleteFails_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<DeleteBankAccountCommandHandler>>();
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

            mockTransactionsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<TransactionDTO>
                {
                    new TransactionDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), TransactionType.Debit.ToString(), 100, "test", "test", DateTime.Now)
                });

            mockTransactionsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var handler = new DeleteBankAccountCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new DeleteBankAccountCommand(Guid.NewGuid()));

            // Assert
            var errorMessage = result.Match(
                                 success => null,
                                 error => error.Message);

            errorMessage.Should().Be("Failed to delete transaction");
        }

        [Fact]
        public async Task DeleteBankAccountCommandHandler_WhenVaultDeleteFails_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<DeleteBankAccountCommandHandler>>();
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

            mockTransactionsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<TransactionDTO>
                {
                    new TransactionDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), TransactionType.Debit.ToString(), 100, "test", "test", DateTime.Now)
                });

            mockTransactionsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockAccountsRepository.Setup(o => o.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));

            mockVaultsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), "test", 100, "GBP"));

            mockVaultsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var handler = new DeleteBankAccountCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new DeleteBankAccountCommand(Guid.NewGuid()));

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);

            errorMessage.Should().Be("Failed to delete vault");
        }

        [Fact]
        public async Task DeleteBankAccountCommandHandler_WhenAccountDeleteFails_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<DeleteBankAccountCommandHandler>>();
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

            mockTransactionsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<TransactionDTO>
                {
                    new TransactionDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), TransactionType.Debit.ToString(), 100, "test", "test", DateTime.Now)
                });

            mockTransactionsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockAccountsRepository.Setup(o => o.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));

            mockVaultsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), "test", 100, "GBP"));

            mockVaultsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockAccountsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var handler = new DeleteBankAccountCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new DeleteBankAccountCommand(Guid.NewGuid()));

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);

            errorMessage.Should().Be("Failed to delete account");
        }

        [Fact]
        public async Task DeleteBankAccountCommandHandler_WhenCustomerAccountDeleteFails_ShouldReturnFailure()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<DeleteBankAccountCommandHandler>>();
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

            mockTransactionsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<TransactionDTO>
                {
                    new TransactionDTO(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), TransactionType.Debit.ToString(), 100, "test", "test", DateTime.Now)
                });

            mockTransactionsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockAccountsRepository.Setup(o => o.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDTO(Guid.NewGuid().ToString(), "Savings", "test", "test"));

            mockVaultsRepository.Setup(o => o.GetByAccountIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new VaultDTO(Guid.NewGuid().ToString(), "test", 100, "GBP"));

            mockVaultsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockAccountsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            mockCustomerAccountsRepository.Setup(o => o.DeleteAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var handler = new DeleteBankAccountCommandHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(new DeleteBankAccountCommand(Guid.NewGuid()));

            // Assert
            var errorMessage = result.Match(
                                      success => null,
                                      error => error.Message);

            errorMessage.Should().Be("Failed to delete customer account");
        }
    }
}
