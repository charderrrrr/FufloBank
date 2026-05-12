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