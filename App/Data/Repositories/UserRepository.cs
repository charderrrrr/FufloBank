using System;
using System.Data;
using Dapper;
using App.Models;

namespace App.Data.Repositories
{
    public class UserRepository
    {
        private readonly IDbConnection _connection;

        public UserRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public UserProfile? GetById(Guid id)
        {
            return _connection.QuerySingleOrDefault<UserProfile>(
                "SELECT * FROM users WHERE id = @Id", new { Id = id });
        }

        public UserProfile? GetByPhone(string phone)
        {
            return _connection.QuerySingleOrDefault<UserProfile>(
                "SELECT * FROM users WHERE phone = @Phone", new { Phone = phone });
        }

        public void Create(UserProfile user)
        {
            _connection.Execute(@"
                INSERT INTO users (id, full_name, phone, status, created_at, monthly_cashback_limit, current_monthly_cashback)
                VALUES (@Id, @FullName, @Phone, @Status, @CreatedAt, @MonthlyCashbackLimit, @CurrentMonthlyCashback)",
                user);
        }

        public void UpdateCashback(Guid userId, decimal cashbackAmount)
        {
            _connection.Execute(@"
                UPDATE users 
                SET current_monthly_cashback = current_monthly_cashback + @CashbackAmount 
                WHERE id = @UserId",
                new { UserId = userId, CashbackAmount = cashbackAmount });
        }
    }
}