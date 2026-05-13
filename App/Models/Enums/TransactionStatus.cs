// Статусы жизненного цикла транзакции
// Pending - ожидает обработки
// Completed - успешно завершена
// Failed - ошибка выполнения
// Reversed - откат транзакции
// PendingConfirmation - ожидает подтверждения по коду безопасности

namespace App.Models.Enums
{
    public enum TransactionStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Reversed = 4,
        PendingConfirmation = 5
    }
}
