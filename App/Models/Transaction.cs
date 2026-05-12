using System;
using App.Models.Enums;

namespace App.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public Guid? FromUserId { get; set; }
        public Guid? ToUserId { get; set; }
        public decimal Amount { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal? ConvertedAmount { get; set; }
        public TransactionCategory Category { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal? CashbackAmount { get; set; }
        public int? MccCode { get; set; }

        public static Transaction Create(
            Guid? fromAccountId,
            Guid? toAccountId,
            Guid? fromUserId,
            Guid? toUserId,
            decimal amount,
            CurrencyType currency,
            TransactionCategory category,
            string? description = null,
            int? mccCode = null)
        {
            return new Transaction
            {
                Id = Guid.NewGuid(),
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Amount = amount,
                Currency = currency,
                Category = category,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Description = description ?? string.Empty,
                MccCode = mccCode
            };
        }

        public void Complete()
        {
            Status = TransactionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void Fail()
        {
            Status = TransactionStatus.Failed;
            CompletedAt = DateTime.UtcNow;
        }

        public void SetPendingConfirmation()
        {
            Status = TransactionStatus.PendingConfirmation;
        }
    }
}