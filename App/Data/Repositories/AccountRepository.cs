using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using App.Models;
using App.Models.Enums;

namespace App.Data.Repositories
{
    public class AccountRepository
    {
        private readonly IDbConnection _connection;

        public AccountRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public Account? GetById(Guid id)
        {
            return _connection.QuerySingleOrDefault<Account>(
                "SELECT * FROM accounts WHERE id = @Id", new { Id = id });
        }

        public IEnumerable<Account> GetByUserId(Guid userId)
        {
            return _connection.Query<Account>(
                "SELECT * FROM accounts WHERE user_id = @UserId AND is_active = TRUE", 
                new { UserId = userId });
        }

        public Account? GetByUserIdAndCurrency(Guid userId, CurrencyType currency)
        {
            return _connection.QuerySingleOrDefault<Account>(
                "SELECT * FROM accounts WHERE user_id = @UserId AND currency = @Currency AND is_active = TRUE",
                new { UserId = userId, Currency = (int)currency });
        }

        public void Create(Account account)
        {
            _connection.Execute(@"
                INSERT INTO accounts (id, user_id, currency, balance, created_at, is_active)
                VALUES (@Id, @UserId, @Currency, @Balance, @CreatedAt, @IsActive)",
                account);
        }

        public void UpdateBalance(Guid accountId, decimal newBalance)
        {
            _connection.Execute(
                "UPDATE accounts SET balance = @Balance WHERE id = @Id",
                new { Id = accountId, Balance = newBalance });
        }
    }
}