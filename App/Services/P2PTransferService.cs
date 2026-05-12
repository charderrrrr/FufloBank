using System;
using System.Data;
using App.Data.Repositories;
using App.Models;
using App.Models.Enums;

namespace App.Services
{
    public class P2PTransferService
    {
        private readonly UserRepository _userRepository;
        private readonly AccountRepository _accountRepository;
        private readonly TransactionRepository _transactionRepository;
        private readonly SecurityService _securityService;
        private readonly IDbConnection _connection;

        public P2PTransferService(
            UserRepository userRepository,
            AccountRepository accountRepository,
            TransactionRepository transactionRepository,
            SecurityService securityService,
            IDbConnection connection)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _securityService = securityService;
            _connection = connection;
        }

        public Transaction InitiateTransfer(
            Guid fromUserId, 
            string toPhone, 
            decimal amount, 
            CurrencyType currency,
            string? confirmationCode = null)
        {
            if (_securityService.RequiresConfirmation(amount) && !_securityService.ValidateConfirmationCode(confirmationCode))
                throw new InvalidOperationException("Invalid confirmation code");

            var sender = _userRepository.GetById(fromUserId);
            if (sender == null)
                throw new InvalidOperationException("Sender not found");

            var receiver = _userRepository.GetByPhone(toPhone);
            if (receiver == null)
                throw new InvalidOperationException("Receiver not found");

            var senderAccount = _accountRepository.GetByUserIdAndCurrency(fromUserId, currency);
            if (senderAccount == null || !senderAccount.CanDebit(amount))
                throw new InvalidOperationException("Insufficient funds");

            var receiverAccount = _accountRepository.GetByUserIdAndCurrency(receiver.Id, currency);
            if (receiverAccount == null)
                throw new InvalidOperationException("Receiver account not found");

            var transaction = Transaction.Create(
                senderAccount.Id, receiverAccount.Id, fromUserId, receiver.Id,
                amount, currency, TransactionCategory.Transfer,
                $"P2P transfer to {receiver.Phone}");

            using var tx = _connection.BeginTransaction();
            try
            {
                senderAccount.Debit(amount);
                _accountRepository.UpdateBalance(senderAccount.Id, senderAccount.Balance);

                receiverAccount.Credit(amount);
                _accountRepository.UpdateBalance(receiverAccount.Id, receiverAccount.Balance);

                _transactionRepository.Create(transaction);
                transaction.Complete();
                _transactionRepository.UpdateStatus(transaction.Id, (int)TransactionStatus.Completed, DateTime.UtcNow);

                tx.Commit();
                return transaction;
            }
            catch
            {
                tx.Rollback();
                transaction.Fail();
                throw;
            }
        }
    }
}