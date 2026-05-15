using System.Data;
using App.Data.Repositories;
using App.Services;
using App.Validators;
using App.Models;
using App.Models.Enums;

namespace App
{
    public class FufloBankModule : IDisposable
    {
        private readonly IDbConnection _connection;
        public IDbConnection Connection => _connection;
        public UserRepository UserRepository { get; }
        public AccountRepository AccountRepository { get; }
        public TransactionRepository TransactionRepository { get; }
        public ExchangeRateRepository ExchangeRateRepository { get; }
        public MccCodeRepository MccCodeRepository { get; }
        public CurrencyConversionService CurrencyConversionService { get; }
        public TransactionService TransactionService { get; }
        public P2PTransferService P2PTransferService { get; }
        public CashbackService CashbackService { get; }
        public SecurityService SecurityService { get; }
        public RateUpdaterService RateUpdaterService { get; }
        public StatementService StatementService { get; }
        public PhoneValidator PhoneValidator { get; }

        public FufloBankModule(IDbConnection connection)
        {
            _connection = connection;

            UserRepository = new UserRepository(_connection);
            AccountRepository = new AccountRepository(_connection);
            TransactionRepository = new TransactionRepository(_connection);
            ExchangeRateRepository = new ExchangeRateRepository(_connection);
            MccCodeRepository = new MccCodeRepository(_connection);
            CurrencyConversionService = new CurrencyConversionService(ExchangeRateRepository);
            CashbackService = new CashbackService(_connection);
            SecurityService = new SecurityService();
            TransactionService = new TransactionService(AccountRepository, TransactionRepository, CashbackService, MccCodeRepository, _connection);
            P2PTransferService = new P2PTransferService(UserRepository, AccountRepository, TransactionRepository, SecurityService, _connection);
            RateUpdaterService = new RateUpdaterService(ExchangeRateRepository);
            StatementService = new StatementService(TransactionRepository);
            PhoneValidator = new PhoneValidator();
        }

        public UserProfile RegisterUser(string fullName, string phone)
        {
            if (!PhoneValidator.Validate(phone))
                throw new ArgumentException("Invalid phone format");

            var existingUser = UserRepository.GetByPhone(phone);
            if (existingUser != null)
                throw new InvalidOperationException("User with this phone already exists");

            var user = UserProfile.Create(fullName, phone);
            UserRepository.Create(user);

            var rubAccount = Account.Create(user.Id, CurrencyType.RUB);
            var usdAccount = Account.Create(user.Id, CurrencyType.USD);
            var cryptoAccount = Account.Create(user.Id, CurrencyType.CRYPTO);

            AccountRepository.Create(rubAccount);
            AccountRepository.Create(usdAccount);
            AccountRepository.Create(cryptoAccount);

            return user;
        }

        public void Deposit(Guid userId, CurrencyType currency, decimal amount)
        {
            var account = AccountRepository.GetByUserIdAndCurrency(userId, currency);
            if (account == null)
                throw new InvalidOperationException($"Account not found for {currency}");

            account.Credit(amount);
            AccountRepository.UpdateBalance(account.Id, account.Balance);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}