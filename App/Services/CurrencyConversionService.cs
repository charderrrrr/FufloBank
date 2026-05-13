// Сервис конвертации валют по актуальному курсу
// _rateRepository - источник курсов валют из БД
// Convert - конвертирует сумму из одной валюты в другую, для одинаковых валют возвращает исходную сумму

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
                throw new InvalidOperationException($"Курс обмена не найден для {from} к {to}");

            return amount * rate.Rate;
        }
    }
}
