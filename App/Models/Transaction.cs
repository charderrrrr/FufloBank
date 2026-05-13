// Модель финансовой транзакции
// FromAccountId - счет отправителя (null для внешних платежей)
// ToAccountId - счет получателя (null для списаний)
// FromUserId - пользователь-отправитель
// ToUserId - пользователь-получатель
// Amount - сумма транзакции в исходной валюте
// Currency - валюта операции
// ConvertedAmount - сконвертированная сумма при мультивалютных операциях
// Category - категория для начисления кэшбэка
// Status - текущий статус транзакции
// CreatedAt - время создания транзакции
// CompletedAt - время завершения обработки
// Description - текстовое описание операции
// CashbackAmount - начисленный кэшбэк
// MccCode - код категории мерчанта для автоопределения кэшбэка
// Create - фабричный метод с инициализацией статуса Pending
// Complete - маркировка транзакции как успешной
// Fail - маркировка транзакции как неудачной
// SetPendingConfirmation - установка статуса ожидания кода подтверждения

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
