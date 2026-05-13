// Сервис обработки платежных транзакций
// _accountRepository - доступ к счетам для списания
// _transactionRepository - сохранение транзакции в историю
// _cashbackService - расчет кэшбэка по MCC коду
// _mccCodeRepository - определение категории по коду мерчанта
// _connection - подключение к БД для обеспечения атомарности через транзакцию
// ProcessPayment - списание со счета, определение категории по MCC, начисление кэшбэка, сохранение в историю

using System;
using System.Data;
using App.Data.Repositories;
using App.Models;
using App.Models.Enums;

namespace App.Services
{
    public class TransactionService
    {
        private readonly AccountRepository _accountRepository;
        private readonly TransactionRepository _transactionRepository;
        private readonly CashbackService _cashbackService;
        private readonly MccCodeRepository _mccCodeRepository;
        private readonly IDbConnection _connection;

        public TransactionService(
            AccountRepository accountRepository,
            TransactionRepository transactionRepository,
            CashbackService cashbackService,
            MccCodeRepository mccCodeRepository,
            IDbConnection connection)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _cashbackService = cashbackService;
            _mccCodeRepository = mccCodeRepository;
            _connection = connection;
        }

        public Transaction ProcessPayment(
            Guid userId, 
            decimal amount, 
            CurrencyType currency, 
            int mccCode,
            string? description = null)
        {
            var mcc = _mccCodeRepository.GetByCode(mccCode);
            var category = mcc?.Category ?? TransactionCategory.Other;

            var account = _accountRepository.GetByUserIdAndCurrency(userId, currency);
            if (account == null || !account.CanDebit(amount))
                throw new InvalidOperationException("Insufficient funds");

            var transaction = Transaction.Create(
                account.Id, null, userId, null,
                amount, currency, category, description, mccCode);

            var cashbackAmount = _cashbackService.CalculateCashback(category, amount);

            using var tx = _connection.BeginTransaction();
            try
            {
                account.Debit(amount);
                _accountRepository.UpdateBalance(account.Id, account.Balance);

                if (cashbackAmount > 0 && _cashbackService.CanAccrueCashback(userId, cashbackAmount))
                {
                    _accountRepository.UpdateBalance(account.Id, account.Balance + cashbackAmount);
                    transaction.CashbackAmount = cashbackAmount;
                }

                _transactionRepository.Create(transaction);
                transaction.Complete();
                _transactionRepository.UpdateStatus(
                    transaction.Id, 
                    (int)TransactionStatus.Completed, 
                    DateTime.UtcNow, 
                    cashbackAmount);

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
