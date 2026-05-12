using System;
using App.Models.Enums;

namespace App.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        public static Account Create(Guid userId, CurrencyType currency)
        {
            return new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Currency = currency,
                Balance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }

        public bool CanDebit(decimal amount)
        {
            return IsActive && Balance >= amount;
        }

        public void Debit(decimal amount)
        {
            if (!CanDebit(amount))
                throw new InvalidOperationException("Insufficient funds or inactive account");
            Balance -= amount;
        }

        public void Credit(decimal amount)
        {
            if (!IsActive)
                throw new InvalidOperationException("Account is inactive");
            Balance += amount;
        }
    }
}