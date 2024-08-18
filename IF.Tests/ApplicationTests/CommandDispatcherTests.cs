using IF.Application.Abstractions;
using IF.Application.AccountService.Commands;
using IF.Application.AccountService.Queries;
using IF.Domain;
using IF.Domain.DTOs;
using IF.Domain.Entities;
using IF.Domain.ErrorMessages;
using Moq;

namespace IF.Tests.ApplicationTests
{
    public class CommandDispatcherTests
    {
        [Fact]
        public async Task DispatchAsync_WithValidCommand_ShouldSucceed()
        {
            // Arrange
            var command = new DeleteAccountCommand(Guid.NewGuid());
            var mockServiceProvider = new Mock<IServiceProvider>();
            var commandHandler = new Mock<ICommandHandler<DeleteAccountCommand, Result<bool, ValidationError>>>();

            commandHandler.Setup(x => x.HandleAsync(It.IsAny<DeleteAccountCommand>()))
                .ReturnsAsync(Result<bool, ValidationError>.Success(true));

            mockServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<DeleteAccountCommand, Result<bool, ValidationError>>)))
                .Returns(commandHandler.Object);

            var dispatcher = new CommandDispatcher(mockServiceProvider.Object);

            // Act
            var result = await dispatcher.DispatchAsync<DeleteAccountCommand, Result<bool, ValidationError>>(command);

            // Assert
            commandHandler.Verify(x => x.HandleAsync(command), Times.Once);
        }

        [Fact]
        public async Task DispatchAsync_WithInvalidCommand_ShouldThrowException()
        {
            // Arrange
            var command = new DeleteAccountCommand(Guid.Empty);
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockCommandHandler = new Mock<ICommandHandler<DeleteAccountCommand, Result<bool, ValidationError>>>();
            
            mockCommandHandler.Setup(x => x.HandleAsync(command))
                .ReturnsAsync(Result<bool, ValidationError>.Failure(new ValidationError("Failed to delete account")));
            
            mockServiceProvider.Setup(x => x.GetService(typeof(ICommandHandler<DeleteAccountCommand, Result<bool, ValidationError>>)))
                .Returns(mockCommandHandler.Object);
            
            var sut = new CommandDispatcher(mockServiceProvider.Object);

            // Act
            var result = await sut.DispatchAsync<DeleteAccountCommand, Result<bool, ValidationError>>(command);

            // Assert
            var errorMessage = result.Match(
                               success => default,
                               error => error.Message);
            Assert.Equal("Failed to delete account", errorMessage);
        }

        [Fact]
        public async Task QueryAsync_WithValidQuery_ShouldSucceed()
        {
            // Arrange
            var query = new GetAccountByIdQuery(Guid.NewGuid());
            var queryHandler = new Mock<IQueryHandler<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>>();
            var mockServiceProvider = new Mock<IServiceProvider>();

            queryHandler.Setup(x => x.HandleAsync(query)).ReturnsAsync(Result<AccountDTO, NotFoundError>.Success(new AccountDTO()));

            mockServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>)))
                .Returns(queryHandler.Object);

            var sut = new CommandDispatcher(mockServiceProvider.Object);

            // Act
            var result = await sut.QueryAsync<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>(query);

            // Assert
            var account = result.Match(
                               success => success,
                               error => default);
            Assert.NotNull(account);
            queryHandler.Verify(x => x.HandleAsync(query), Times.Once);
        }

        [Fact]
        public async Task QueryAsync_WithInvalidQuery_ShouldThrowException()
        {
            // Arrange
            var query = new GetAccountByIdQuery(Guid.Empty);
            var queryHandler = new Mock<IQueryHandler<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>>();
            var mockServiceProvider = new Mock<IServiceProvider>(); 
            var sut = new CommandDispatcher(mockServiceProvider.Object);

            queryHandler.Setup(x => x.HandleAsync(query))
                .ReturnsAsync(Result<AccountDTO, NotFoundError>.Failure(new NotFoundError("Account not found")));

            mockServiceProvider.Setup(x => x.GetService(typeof(IQueryHandler<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>)))
                .Returns(queryHandler.Object);

            // Act
            var result = await sut.QueryAsync<GetAccountByIdQuery, Result<AccountDTO, NotFoundError>>(query);

            // Assert
            var errorMessage = result.Match(
                                     success => default,
                                     error => error.Message);
            Assert.Equal("Account not found", errorMessage);
        }
    }
}
