using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using App.Models.Enums;
using App.UI.Services;

namespace App.Controllers
{
    [ApiController]
    [Route("api/balance")]
    public class BalanceController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        public BalanceController(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public IActionResult GetBalances()
        {
            if (!_sessionManager.IsAuthenticated || _sessionManager.CurrentUser == null)
                return Unauthorized();

            var balances = new List<BalanceResponse>();
            var userId = _sessionManager.CurrentUser.Id;

            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                var account = _sessionManager.BankModule.AccountRepository
                    .GetByUserIdAndCurrency(userId, currency);

                if (account != null)
                {
                    balances.Add(new BalanceResponse
                    {
                        Currency = currency.ToString(),
                        Balance = account.Balance
                    });
                }
            }

            return Ok(balances);
        }
    }

    public class BalanceResponse
    {
        public string Currency { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}