// Валидатор лимитов безопасности для P2P переводов
// ConfirmationThreshold - пороговая сумма 50000 RUB, требующая дополнительного подтверждения
// RequiresConfirmation - проверяет, превышает ли сумма транзакции установленный лимит

namespace App.Validators
{
    public class TransferLimitValidator
    {
        private const decimal ConfirmationThreshold = 50000.00m;

        public bool RequiresConfirmation(decimal amount)
        {
            return amount > ConfirmationThreshold;
        }
    }
}
