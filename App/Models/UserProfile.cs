using System;
using App.Models.Enums;

namespace App.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal MonthlyCashbackLimit { get; set; }
        public decimal CurrentMonthlyCashback { get; set; }

        public static UserProfile Create(string fullName, string phone)
        {
            return new UserProfile
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Phone = phone,
                Status = UserStatus.NotVerified,
                CreatedAt = DateTime.UtcNow,
                MonthlyCashbackLimit = 5000.00m,
                CurrentMonthlyCashback = 0
            };
        }
    }
}