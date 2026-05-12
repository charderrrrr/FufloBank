using System;
using App.Models.Enums;

namespace App.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public CurrencyType FromCurrency { get; set; }
        public CurrencyType ToCurrency { get; set; }
        public decimal Rate { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static ExchangeRate Create(CurrencyType from, CurrencyType to, decimal rate)
        {
            return new ExchangeRate
            {
                FromCurrency = from,
                ToCurrency = to,
                Rate = rate,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}