using System.Data;
using Dapper;
using App.Models;
using App.Models.Enums;

namespace App.Data.Repositories
{
    public class ExchangeRateRepository
    {
        private readonly IDbConnection _connection;

        public ExchangeRateRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public ExchangeRate? GetRate(CurrencyType from, CurrencyType to)
        {
            return _connection.QuerySingleOrDefault<ExchangeRate>(
                "SELECT * FROM exchange_rates WHERE from_currency = @From AND to_currency = @To ORDER BY updated_at DESC LIMIT 1",
                new { From = (int)from, To = (int)to });
        }

        public void UpsertRate(ExchangeRate rate)
        {
            _connection.Execute(@"
                INSERT INTO exchange_rates (from_currency, to_currency, rate, updated_at)
                VALUES (@FromCurrency, @ToCurrency, @Rate, @UpdatedAt)
                ON CONFLICT (from_currency, to_currency) DO UPDATE 
                SET rate = @Rate, updated_at = @UpdatedAt",
                rate);
        }
    }
}