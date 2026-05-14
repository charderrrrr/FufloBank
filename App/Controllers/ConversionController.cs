using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using App.Models.Enums;
using App.UI.Services;

namespace App.Controllers
{
    [ApiController]
    [Route("api/conversion")]
    public class ConversionController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        public ConversionController(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpPost]
        public IActionResult Convert([FromBody] ConversionRequest request)
        {
            if (!_sessionManager.IsAuthenticated)
                return Unauthorized();

            try
            {
                var convertedAmount = _sessionManager.BankModule.CurrencyConversionService.Convert(
                    request.Amount,
                    request.FromCurrency,
                    request.ToCurrency);

                return Ok(new ConversionResponse
                {
                    FromCurrency = request.FromCurrency.ToString(),
                    ToCurrency = request.ToCurrency.ToString(),
                    OriginalAmount = request.Amount,
                    ConvertedAmount = convertedAmount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("rates")]
        public IActionResult GetRates()
        {
            if (!_sessionManager.IsAuthenticated)
                return Unauthorized();

            var rates = new List<RateResponse>();

            foreach (CurrencyType from in Enum.GetValues(typeof(CurrencyType)))
            {
                foreach (CurrencyType to in Enum.GetValues(typeof(CurrencyType)))
                {
                    if (from != to)
                    {
                        var rate = _sessionManager.BankModule.ExchangeRateRepository.GetRate(from, to);
                        if (rate != null)
                        {
                            rates.Add(new RateResponse
                            {
                                FromCurrency = from.ToString(),
                                ToCurrency = to.ToString(),
                                Rate = rate.Rate
                            });
                        }
                    }
                }
            }

            return Ok(rates);
        }
    }

    public class ConversionRequest
    {
        public CurrencyType FromCurrency { get; set; }
        public CurrencyType ToCurrency { get; set; }
        public decimal Amount { get; set; }
    }

    public class ConversionResponse
    {
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
        public decimal OriginalAmount { get; set; }
        public decimal ConvertedAmount { get; set; }
    }

    public class RateResponse
    {
        public string FromCurrency { get; set; } = string.Empty;
        public string ToCurrency { get; set; } = string.Empty;
        public decimal Rate { get; set; }
    }
}