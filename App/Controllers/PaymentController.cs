using System;
using Microsoft.AspNetCore.Mvc;
using App.Models.Enums;
using App.UI.Services;

namespace App.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        public PaymentController(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpPost]
        public IActionResult MakePayment([FromBody] PaymentRequest request)
        {
            if (!_sessionManager.IsAuthenticated || _sessionManager.CurrentUser == null)
                return Unauthorized();

            try
            {
                var transaction = _sessionManager.BankModule.TransactionService.ProcessPayment(
                    _sessionManager.CurrentUser.Id,
                    request.Amount,
                    request.Currency,
                    request.MccCode,
                    request.Description);

                return Ok(new PaymentResponse
                {
                    TransactionId = transaction.Id,
                    Amount = transaction.Amount,
                    CashbackAmount = transaction.CashbackAmount,
                    Status = transaction.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class PaymentRequest
    {
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public int MccCode { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class PaymentResponse
    {
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public decimal? CashbackAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}