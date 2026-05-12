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