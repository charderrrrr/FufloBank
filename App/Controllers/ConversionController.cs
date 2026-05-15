using System;
using System.Data;
using System.Text.Json.Serialization;
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
        private readonly IDbConnection _connection;

        public ConversionController(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _connection = sessionManager.BankModule.Connection;
        }

        [HttpPost]
        public IActionResult Convert([FromBody] ConversionRequest request)
        {
            if (!_sessionManager.IsAuthenticated || _sessionManager.CurrentUser == null)
                return Unauthorized();

            var fromAccount = _sessionManager.BankModule.AccountRepository
                .GetByUserIdAndCurrency(_sessionManager.CurrentUser.Id, request.FromCurrency);

            var toAccount = _sessionManager.BankModule.AccountRepository
                .GetByUserIdAndCurrency(_sessionManager.CurrentUser.Id, request.ToCurrency);

            if (fromAccount == null || toAccount == null)
                return BadRequest(new { error = "Счет не найден" });

            if (!fromAccount.CanDebit(request.Amount))
                return BadRequest(new { error = "Мало деняк . _." });

            try
            {
                var convertedAmount = _sessionManager.BankModule.CurrencyConversionService.Convert(
                    request.Amount, request.FromCurrency, request.ToCurrency);

                using var tx = _connection.BeginTransaction();
                try
                {
                    fromAccount.Debit(request.Amount);
                    _sessionManager.BankModule.AccountRepository.UpdateBalance(fromAccount.Id, fromAccount.Balance);

                    toAccount.Credit(convertedAmount);
                    _sessionManager.BankModule.AccountRepository.UpdateBalance(toAccount.Id, toAccount.Balance);

                    var transaction = Models.Transaction.Create(
                        fromAccount.Id, toAccount.Id,
                        _sessionManager.CurrentUser.Id, _sessionManager.CurrentUser.Id,
                        request.Amount, request.FromCurrency,
                        TransactionCategory.Conversion,
                        $"Из {request.FromCurrency} в {request.ToCurrency}");

                    transaction.ConvertedAmount = convertedAmount;
                    _sessionManager.BankModule.TransactionRepository.Create(transaction);
                    transaction.Complete();
                    _sessionManager.BankModule.TransactionRepository.UpdateStatus(
                        transaction.Id, (int)TransactionStatus.Completed, DateTime.UtcNow);

                    tx.Commit();

                    return Ok(new ConversionResponse
                    {
                        FromCurrency = request.FromCurrency.ToString(),
                        ToCurrency = request.ToCurrency.ToString(),
                        OriginalAmount = request.Amount,
                        ConvertedAmount = convertedAmount
                    });
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class ConversionRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CurrencyType FromCurrency { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
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
}