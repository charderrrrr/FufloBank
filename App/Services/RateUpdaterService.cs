// Сервис обновления курсов валют из внешнего источника
// _rateRepository - сохранение курсов в БД
// UpdateRates - загрузка курсов из JSON файла с десериализацией, автосоздание файла с дефолтными значениями при отсутствии
// CreateDefaultRatesFile - генерация файла rates.json с базовыми курсами RUB/USD/CRYPTO при первом запуске

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using App.Data.Repositories;
using App.Models;
using App.Models.Enums;

namespace App.Services
{
    public class RateUpdaterService
    {
        private readonly ExchangeRateRepository _rateRepository;

        public RateUpdaterService(ExchangeRateRepository rateRepository)
        {
            _rateRepository = rateRepository;
        }

        public void UpdateRates(string ratesFilePath = "rates.json")
        {
            if (!File.Exists(ratesFilePath))
            {
                CreateDefaultRatesFile(ratesFilePath);
            }

            var jsonContent = File.ReadAllText(ratesFilePath);
            var ratesData = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonContent);

            if (ratesData == null)
                throw new InvalidOperationException("Failed to deserialize rates data");

            foreach (var rateData in ratesData)
            {
                var currencies = rateData.Key.Split('_');
                if (currencies.Length != 2)
                    continue;

                if (Enum.TryParse<CurrencyType>(currencies[0], out var from) &&
                    Enum.TryParse<CurrencyType>(currencies[1], out var to))
                {
                    var rate = ExchangeRate.Create(from, to, rateData.Value);
                    _rateRepository.UpsertRate(rate);
                }
            }
        }

        private void CreateDefaultRatesFile(string path)
        {
            var defaultRates = new Dictionary<string, decimal>
            {
                { "RUB_USD", 0.011m },
                { "USD_RUB", 90.50m },
                { "RUB_CRYPTO", 0.00000025m },
                { "CRYPTO_RUB", 4000000.00m },
                { "USD_CRYPTO", 0.000023m },
                { "CRYPTO_USD", 44000.00m }
            };

            var json = JsonSerializer.Serialize(defaultRates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }
}
