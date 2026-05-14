using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using App.UI.Services;

namespace App.Controllers
{
    [ApiController]
    [Route("api/history")]
    public class HistoryController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        public HistoryController(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public IActionResult GetHistory(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] string filter = "all",
            [FromQuery] int? category = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null)
        {
            if (!_sessionManager.IsAuthenticated || _sessionManager.CurrentUser == null)
                return Unauthorized();

            bool? isIncoming = filter switch
            {
                "incoming" => true,
                "outgoing" => false,
                _ => null
            };

            var currentUserId = _sessionManager.CurrentUser.Id;

            var transactions = _sessionManager.BankModule.StatementService.GetStatement(
                currentUserId,
                fromDate,
                toDate,
                minAmount,
                maxAmount,
                category,
                isIncoming);

            var result = transactions.Select(t => new TransactionHistoryResponse
            {
                Id = t.Id,
                Date = t.CreatedAt,
                Type = t.FromUserId == currentUserId ? "Outgoing" : "Incoming",
                Amount = t.Amount,
                Currency = t.Currency.ToString(),
                Category = t.Category.ToString(),
                Status = t.Status.ToString(),
                Cashback = t.CashbackAmount,
                Description = t.Description
            });

            return Ok(result);
        }
    }

    public class TransactionHistoryResponse
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? Cashback { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}