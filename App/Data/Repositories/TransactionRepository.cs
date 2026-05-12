using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using App.Models;

namespace App.Data.Repositories
{
    public class TransactionRepository
    {
        private readonly IDbConnection _connection;

        public TransactionRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Create(Transaction transaction)
        {
            _connection.Execute(@"
                INSERT INTO transactions (id, from_account_id, to_account_id, from_user_id, to_user_id, 
                    amount, currency, converted_amount, category, status, created_at, completed_at, 
                    description, cashback_amount, mcc_code)
                VALUES (@Id, @FromAccountId, @ToAccountId, @FromUserId, @ToUserId, 
                    @Amount, @Currency, @ConvertedAmount, @Category, @Status, @CreatedAt, @CompletedAt,
                    @Description, @CashbackAmount, @MccCode)",
                transaction);
        }

        public void UpdateStatus(Guid transactionId, int status, DateTime? completedAt = null, decimal? cashbackAmount = null)
        {
            _connection.Execute(@"
                UPDATE transactions 
                SET status = @Status, completed_at = @CompletedAt, cashback_amount = COALESCE(@CashbackAmount, cashback_amount)
                WHERE id = @Id",
                new { Id = transactionId, Status = status, CompletedAt = completedAt ?? DateTime.UtcNow, CashbackAmount = cashbackAmount });
        }

        public IEnumerable<Transaction> GetStatement(Guid userId, DateTime from, DateTime to, 
            decimal? minAmount = null, decimal? maxAmount = null, int? category = null, bool? isIncoming = null)
        {
            var sql = @"
                SELECT * FROM transactions 
                WHERE (from_user_id = @UserId OR to_user_id = @UserId)
                AND created_at BETWEEN @From AND @To
                AND (@MinAmount IS NULL OR amount >= @MinAmount)
                AND (@MaxAmount IS NULL OR amount <= @MaxAmount)
                AND (@Category IS NULL OR category = @Category)
                AND (@IsIncoming IS NULL OR 
                    (@IsIncoming = TRUE AND to_user_id = @UserId) OR 
                    (@IsIncoming = FALSE AND from_user_id = @UserId))
                ORDER BY created_at DESC";

            return _connection.Query<Transaction>(sql, new 
            { 
                UserId = userId, From = from, To = to,
                MinAmount = minAmount, MaxAmount = maxAmount,
                Category = category, IsIncoming = isIncoming
            });
        }
    }
}