// Модель маппинга MCC кодов на категории кэшбэка
// Code - уникальный код категории мерчанта
// Category - соответствующая категория транзакции
// Description - текстовое описание MCC кода

using App.Models.Enums;

namespace App.Models
{
    public class MccCode
    {
        public int Code { get; set; }
        public TransactionCategory Category { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
