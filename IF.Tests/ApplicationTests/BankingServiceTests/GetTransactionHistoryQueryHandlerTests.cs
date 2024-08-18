using FluentAssertions;
using IF.Application.BankingService.Queries;
using IF.Application.BankingService.QueryHandler;
using IF.Domain.DTOs;
using IF.Domain.Enums;
using IF.Domain.ValueObjects;
using IF.Infrastructure;
using IF.Infrastructure.BankingRepository;
using Microsoft.Extensions.Logging;
using Moq;

namespace IF.Tests.ApplicationTests.BankingServiceTests
{
    public class GetTransactionHistoryQueryHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenGetTransactionHistoryIsSuccessful_ShouldReturnTransactionHistory()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<GetTransactionHistoryQueryHandler>>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);
            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);

            var transactions = new List<TransactionDTO>
            {
                new TransactionDTO(
                    Guid.NewGuid().ToString(),
                    Guid.NewGuid().ToString(),
                    TransactionType.TransferSent.ToString(),
                    100,
                    "USD",
                    "test",
                    DateTime.UtcNow)
            };

            var customerAccounts = new List<CustomerAccountDTO>
            {
                new CustomerAccountDTO()
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = Guid.NewGuid().ToString(),
                    CustomerId = Guid.NewGuid().ToString(),
                }
            };

            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(customerAccounts);
            mockTransactions.Setup(x => x.GetByAccountIdAsync(It.IsAny<Guid>())).ReturnsAsync(transactions);

            var query = new GetTransactionHistoryQuery(Guid.NewGuid());

            var handler = new GetTransactionHistoryQueryHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            var transactionHistory = result.Match(
                               transactionHistory => transactionHistory,
                               error => null);
            transactionHistory.Count().Should().Be(1);
        }

        [Fact]
        public async Task HandleAsync_WhenGetTransactionHistoryIsNotSuccessful_ShouldReturnNotFound()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<GetTransactionHistoryQueryHandler>>();
            var mockCustomerAccounts = new Mock<ICustomerAccountRepository>();
            var mockTransactions = new Mock<ITransactionRepository>();

            mockUnitOfWork.Setup(x => x.Transactions).Returns(mockTransactions.Object);
            mockUnitOfWork.Setup(x => x.CustomerAccounts).Returns(mockCustomerAccounts.Object);

            var customerAccounts = new List<CustomerAccountDTO>();

            mockCustomerAccounts.Setup(x => x.GetByCustomerIdAsync(It.IsAny<Guid>())).ReturnsAsync(customerAccounts);

            var query = new GetTransactionHistoryQuery(Guid.NewGuid());

            var handler = new GetTransactionHistoryQueryHandler(mockUnitOfWork.Object, mockLogger.Object);

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            var errorMessage = result.Match(
                               transactionHistory => null,
                               error => error.Message);
            errorMessage.Should().Be("Transaction history not found");
        }
    }
}
