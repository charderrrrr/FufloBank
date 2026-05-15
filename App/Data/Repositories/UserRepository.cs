// Репозиторий для работы с таблицей users
// GetById - поиск пользователя по GUID
// GetByPhone - поиск пользователя по номеру телефона для авторизации и P2P переводов
// Create - регистрация нового пользователя в БД
// UpdateCashback - обновление накопленного кэшбэка за месяц

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

        public UserProfile? GetByPhoneAndPassword(string phone, string passwordHash)
        {
            return _connection.QuerySingleOrDefault<UserProfile>(
                "SELECT * FROM users WHERE phone = @Phone AND password_hash = @PasswordHash",
                new { Phone = phone, PasswordHash = passwordHash });
        }

        public void Create(UserProfile user)
        {
            _connection.Execute(@"
                INSERT INTO users (id, full_name, phone, password_hash, status, created_at, monthly_cashback_limit, current_monthly_cashback)
                VALUES (@Id, @FullName, @Phone, @PasswordHash, @Status, @CreatedAt, @MonthlyCashbackLimit, @CurrentMonthlyCashback)",
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