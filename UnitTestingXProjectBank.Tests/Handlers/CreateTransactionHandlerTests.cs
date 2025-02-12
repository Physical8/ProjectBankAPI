using Xunit;
using Moq;
using ProjectBankAPI.Application.Commands.Transactions;
using ProjectBankAPI.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ProjectBankAPI.Infrastructure.Persistence.Repositories;

namespace ProjectBankAPI.Tests.Handlers
{
    public class CreateTransactionHandlerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IBankAccountRepository> _mockBankAccountRepository;
        private readonly CreateTransactionHandler _handler;

        public CreateTransactionHandlerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockBankAccountRepository = new Mock<IBankAccountRepository>();

            _handler = new CreateTransactionHandler(_mockTransactionRepository.Object, _mockBankAccountRepository.Object);
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_AccountNotFound()
        {
            // Arrange
            var request = new CreateTransactionCommand
            {
                AccountId = 99,
                Amount = 500,
                Type = TransactionType.Withdrawal
            };

            _mockBankAccountRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(default(BankAccount));

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("La cuenta bancaria con ID 99 no existe.");
        }

        [Fact]
        public async Task Handle_Should_ThrowException_When_InsufficientBalance()
        {
            // Arrange
            var account = new BankAccount { Id = 1, Balance = 100 };

            var request = new CreateTransactionCommand
            {
                AccountId = 1,
                Amount = 500,
                Type = TransactionType.Withdrawal
            };

            _mockBankAccountRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(account);

            // Act
            Func<Task> act = async () => await _handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Saldo insuficiente para retiro.");
        }

        [Fact]
        public async Task Handle_Should_DecreaseBalance_When_WithdrawalSuccessful()
        {
            // Arrange
            var account = new BankAccount { Id = 1, Balance = 1000 };

            var request = new CreateTransactionCommand
            {
                AccountId = 1,
                Amount = 200,
                Type = TransactionType.Withdrawal
            };

            _mockBankAccountRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(account);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Amount.Should().Be(200);
            account.Balance.Should().Be(800); // ✅ Verifica que el saldo se redujo correctamente
        }
    }
}
