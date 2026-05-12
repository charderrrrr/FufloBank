using System;
using App.Data.Repositories;
using App.Models.Enums;

namespace App.Services
{
    public class CurrencyConversionService
    {
        private readonly ExchangeRateRepository _rateRepository;

        public CurrencyConversionService(ExchangeRateRepository rateRepository)
        {
            _rateRepository = rateRepository;
        }

        public decimal Convert(decimal amount, CurrencyType from, CurrencyType to)
        {
            if (from == to)
                return amount;

            var rate = _rateRepository.GetRate(from, to);
            if (rate == null)
                throw new InvalidOperationException($"Exchange rate not found for {from} to {to}");

            return amount * rate.Rate;
        }
    }
}