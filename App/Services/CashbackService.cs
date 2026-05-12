using System;
using System.Data;
using System.Linq;
using Dapper;
using App.Models.Enums;

namespace App.Services
{
    public class CashbackService
    {
        private readonly IDbConnection _connection;

        public CashbackService(IDbConnection connection)
        {
            _connection = connection;
        }

        public decimal CalculateCashback(TransactionCategory category, decimal amount)
        {
            var cashbackCategory = _connection.QuerySingleOrDefault<dynamic>(
                "SELECT percentage FROM cashback_categories WHERE category = @Category AND is_active = TRUE",
                new { Category = (int)category });

            if (cashbackCategory == null)
                return 0;

            decimal percentage = (decimal)cashbackCategory.percentage;
            return Math.Round(amount * (percentage / 100), 2);
        }

        public bool CanAccrueCashback(Guid userId, decimal cashbackAmount)
        {
            var user = _connection.QuerySingleOrDefault<dynamic>(
                "SELECT monthly_cashback_limit, current_monthly_cashback FROM users WHERE id = @Id",
                new { Id = userId });

            if (user == null)
                return false;

            decimal limit = (decimal)user.monthly_cashback_limit;
            decimal current = (decimal)user.current_monthly_cashback;

            return (current + cashbackAmount) <= limit;
        }
    }
}