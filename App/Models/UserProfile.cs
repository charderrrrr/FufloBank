// Модель профиля пользователя системы
// Id - уникальный идентификатор пользователя
// FullName - полное имя владельца аккаунта
// Phone - номер телефона в формате +7XXXXXXXXXX (уникальный)
// Status - статус верификации аккаунта
// CreatedAt - дата и время регистрации
// MonthlyCashbackLimit - месячный лимит кэшбэка (по умолчанию 5000)
// CurrentMonthlyCashback - накопленный кэшбэк за текущий месяц
// Create - фабричный метод создания профиля с дефолтными значениями

using System;
using App.Models.Enums;

namespace App.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal MonthlyCashbackLimit { get; set; }
        public decimal CurrentMonthlyCashback { get; set; }

        public static UserProfile Create(string fullName, string phone, string passwordHash)
        {
            return new UserProfile
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Phone = phone,
                PasswordHash = passwordHash,
                Status = UserStatus.NotVerified,
                CreatedAt = DateTime.UtcNow,
                MonthlyCashbackLimit = 5000.00m,
                CurrentMonthlyCashback = 0
            };
        }
    }
}