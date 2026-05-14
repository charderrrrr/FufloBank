using System;
using Microsoft.AspNetCore.Mvc;
using App.Models.Enums;
using App.UI.Services;

namespace App.Controllers
{
    [ApiController]
    [Route("api/transfer")]
    public class TransferController : ControllerBase
    {
        private readonly SessionManager _sessionManager;

        public TransferController(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpPost]
        public IActionResult MakeTransfer([FromBody] TransferRequest request)
        {
            if (!_sessionManager.IsAuthenticated || _sessionManager.CurrentUser == null)
                return Unauthorized();

            try
            {
                var transaction = _sessionManager.BankModule.P2PTransferService.InitiateTransfer(
                    _sessionManager.CurrentUser.Id,
                    request.RecipientPhone,
                    request.Amount,
                    request.Currency,
                    request.ConfirmationCode);

                return Ok(new TransferResponse
                {
                    TransactionId = transaction.Id,
                    Amount = transaction.Amount,
                    RecipientPhone = request.RecipientPhone,
                    Status = transaction.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("check-limit")]
        public IActionResult CheckLimit([FromQuery] decimal amount)
        {
            if (!_sessionManager.IsAuthenticated)
                return Unauthorized();

            var requiresConfirmation = _sessionManager.BankModule.SecurityService.RequiresConfirmation(amount);
            return Ok(new { requiresConfirmation });
        }
    }

    public class TransferRequest
    {
        public string RecipientPhone { get; set; } = string.Empty;
        public CurrencyType Currency { get; set; }
        public decimal Amount { get; set; }
        public string? ConfirmationCode { get; set; }
    }

    public class TransferResponse
    {
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string RecipientPhone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}