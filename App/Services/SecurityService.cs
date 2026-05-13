// Сервис безопасности для проверки лимитов и кодов подтверждения
// _limitValidator - валидатор пороговой суммы
// ConfirmationCode - статический код 1234 для подтверждения крупных переводов
// RequiresConfirmation - проверяет необходимость ввода кода по сумме
// ValidateConfirmationCode - сверяет введенный код с эталонным

using App.Validators;

namespace App.Services
{
    public class SecurityService
    {
        private readonly TransferLimitValidator _limitValidator;
        private const string ConfirmationCode = "1234";

        public SecurityService()
        {
            _limitValidator = new TransferLimitValidator();
        }

        public bool RequiresConfirmation(decimal amount)
        {
            return _limitValidator.RequiresConfirmation(amount);
        }

        public bool ValidateConfirmationCode(string? code)
        {
            return code == ConfirmationCode;
        }
    }
}
